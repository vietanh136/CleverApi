using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CleverApi.Services;
using CleverApi.Models;
using CleverApi.Providers;
using System.Web;

namespace CleverApi.ApiControllers
{
    public class FrequentlyAskedQuestionsController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListQuestion(int? page, int? rowPerPage)
        {
            try
            {
                FrequentlyAskedQuestionsService frequentlyAskedQuestionsService = new FrequentlyAskedQuestionsService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(frequentlyAskedQuestionsService.GetListQuestion(page.Value, rowPerPage.Value)); ;
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetQuestion(string id)
        {
            try
            {
                FrequentlyAskedQuestionsService frequentlyAskedQuestionsService = new FrequentlyAskedQuestionsService();
                C600 question = frequentlyAskedQuestionsService.GetQuestionById(id);
                if (question == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                List<C601> listSteps = frequentlyAskedQuestionsService.GetListSteps(question.ID);
                List<C602> listWhatNeed = frequentlyAskedQuestionsService.GetListWhatNeed(question.ID);
                return Success(new { question, listSteps, listWhatNeed });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult CreateQuestion(FAQUpdateModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Answer)) throw new Exception("Answer cannot be left blank");
                if (string.IsNullOrEmpty(model.Question)) throw new Exception("Question cannot be left blank");
                if (string.IsNullOrEmpty(model.FormFill)) throw new Exception("Form fillcannot be left blank");
                if (string.IsNullOrEmpty(model.ProcessTime)) throw new Exception("Processing Time cannot be left blank");

                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        FrequentlyAskedQuestionsService frequentlyAskedQuestionsService = new FrequentlyAskedQuestionsService(connection);

                        C600 question = new C600();
                        question.Answer = model.Answer;
                        question.CreatedBy = userAdmin.ID;
                        question.FormFill = model.FormFill;
                        question.ID = Guid.NewGuid().ToString();
                        question.Payment = model.Payment;
                        question.ProcessTime = model.ProcessTime;
                        question.Question = model.Question;
                        question.Status = C600.EnumStatus.ENABLED;
                        if (frequentlyAskedQuestionsService.CheckQuestionExist(question,transaction)) throw new Exception("Question existed");
                        if (!string.IsNullOrEmpty(model.Icon))
                        {
                            string fileName = Guid.NewGuid().ToString() + ".png";
                            string path = HttpContext.Current.Server.MapPath(Constant.FAQ_THUMBNAIL_PATH + fileName);
                            HelperProvider.Base64ToImage(model.Icon, path);
                            question.Icon = Constant.FAQ_THUMBNAIL_URL + fileName;
                        }
                        if (!frequentlyAskedQuestionsService.InsertQuestion(question, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        for (int indexStep = 0; indexStep < model.ListSteps.Count; indexStep++)
                        {
                            model.ListSteps[indexStep].FAQID = question.ID;
                            model.ListSteps[indexStep].ID = Guid.NewGuid().ToString();
                            if (!frequentlyAskedQuestionsService.InsertStep(model.ListSteps[indexStep], transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }
                        for (int indexStep = 0; indexStep < model.ListWhatNeed.Count; indexStep++)
                        {
                            model.ListWhatNeed[indexStep].FAQID = question.ID;
                            model.ListWhatNeed[indexStep].ID = Guid.NewGuid().ToString();
                            if (!frequentlyAskedQuestionsService.InsertWhatNeed(model.ListWhatNeed[indexStep], transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }

                        LogProvider.InsertLog("Thêm FAQ", userAdmin, connection, transaction);

                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult UpdateQuestion(FAQUpdateModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Answer)) throw new Exception("Answer cannot be left blank");
                if (string.IsNullOrEmpty(model.Question)) throw new Exception("Question cannot be left blank");
                if (string.IsNullOrEmpty(model.FormFill)) throw new Exception("Form fillcannot be left blank");
                if (string.IsNullOrEmpty(model.ProcessTime)) throw new Exception("Processing Time cannot be left blank");

                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        FrequentlyAskedQuestionsService frequentlyAskedQuestionsService = new FrequentlyAskedQuestionsService(connection);

                        C600 question = frequentlyAskedQuestionsService.GetQuestionById(model.ID, transaction);
                        if (question == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                        question.Answer = model.Answer;
                        question.ModifiedBy = userAdmin.ID;
                        question.FormFill = model.FormFill;
                        question.Payment = model.Payment;
                        question.ProcessTime = model.ProcessTime;
                        question.Question = model.Question;
                        if (frequentlyAskedQuestionsService.CheckQuestionExist(question,transaction)) throw new Exception("Question existed");
                        if (!string.IsNullOrEmpty(model.Icon))
                        {
                            if (!string.IsNullOrEmpty(question.Icon))
                            {
                                HelperProvider.DeleteFile(question.Icon);
                            }
                            string fileName = Guid.NewGuid().ToString() + ".png";
                            string path = HttpContext.Current.Server.MapPath(Constant.FAQ_THUMBNAIL_PATH + fileName);
                            HelperProvider.Base64ToImage(model.Icon, path);
                            question.Icon = Constant.FAQ_THUMBNAIL_URL + fileName;
                        }
                        if (!frequentlyAskedQuestionsService.UpdateQuestion(question, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        for (int indexStep = 0; indexStep < model.ListSteps.Count; indexStep++)
                        {
                            if (string.IsNullOrEmpty(model.ListSteps[indexStep].ID))
                            {
                                model.ListSteps[indexStep].FAQID = question.ID;
                                model.ListSteps[indexStep].ID = Guid.NewGuid().ToString();
                                if (!frequentlyAskedQuestionsService.InsertStep(model.ListSteps[indexStep], transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                            }
                            else
                            {
                                C601 step = frequentlyAskedQuestionsService.GetStepById(model.ListSteps[indexStep].ID, transaction);
                                if (step == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                                step.Description = model.ListSteps[indexStep].Description;
                                step.Note = model.ListSteps[indexStep].Note;
                                step.SortOrder = model.ListSteps[indexStep].SortOrder;
                                if (!frequentlyAskedQuestionsService.UpdateStep(step, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                            }
                        }
                        for (int indexStep = 0; indexStep < model.ListWhatNeed.Count; indexStep++)
                        {
                            if (string.IsNullOrEmpty(model.ListWhatNeed[indexStep].ID))
                            {
                                model.ListWhatNeed[indexStep].FAQID = question.ID;
                                model.ListWhatNeed[indexStep].ID = Guid.NewGuid().ToString();
                                if (!frequentlyAskedQuestionsService.InsertWhatNeed(model.ListWhatNeed[indexStep], transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                            }
                            else
                            {
                                C602 whatNeed = frequentlyAskedQuestionsService.GetWhatNeedById(model.ListWhatNeed[indexStep].ID, transaction);
                                if (whatNeed == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                                whatNeed.Description = model.ListWhatNeed[indexStep].Description;
                                if (!frequentlyAskedQuestionsService.UpdateWhatNeed(whatNeed, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                            }
                        }

                        foreach (var item in model.ListStepsDelete)
                        {
                            C601 step = frequentlyAskedQuestionsService.GetStepById(item, transaction);
                            if (step == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                            if (!frequentlyAskedQuestionsService.DeleteStep(step.ID, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }
                        foreach (var item in model.ListWhatNeedDelete)
                        {
                            C602 whatNeed = frequentlyAskedQuestionsService.GetWhatNeedById(item, transaction);
                            if (whatNeed == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                            if (!frequentlyAskedQuestionsService.DeleteWhatNeed(whatNeed.ID, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }

                        LogProvider.InsertLog("Cập nhật FAQ", userAdmin, connection, transaction);

                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult DeleteQuestion(string id)
        {
            try
            {

                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        FrequentlyAskedQuestionsService frequentlyAskedQuestionsService = new FrequentlyAskedQuestionsService(connection);

                        C600 faq = frequentlyAskedQuestionsService.GetQuestionById(id, transaction);
                        if (faq == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);

                        faq.Status = C600.EnumStatus.DISABLED;
                        faq.ModifiedBy = userAdmin.ID;
                        if (!frequentlyAskedQuestionsService.UpdateQuestion(faq, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        transaction.Commit();
                        return Success();
                    }
                }

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
