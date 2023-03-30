using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CleverApi.Services;
using CleverApi.Models;
using CleverApi.Providers;

namespace CleverApi.ApiControllers
{
    public class BusinessAccountQuestionsController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListQuestion(int? page,int? rowPerPage) {
            try {
                BusinessAccountQuestionsService businessAccountQuestionsService = new BusinessAccountQuestionsService();
                if (!page.HasValue) page = 1;
                if (!rowPerPage.HasValue) rowPerPage = Constant.ADMIN_PAGE_SIZE;
                return Success(businessAccountQuestionsService.GetListQuestion(page.Value, rowPerPage.Value));
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetQuestion(string id) {
            try
            {
                BusinessAccountQuestionsService businessAccountQuestionsService = new BusinessAccountQuestionsService();
                C500 question = businessAccountQuestionsService.GetQuestionById(id);
                if (question == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                List<C501> listAnswer = businessAccountQuestionsService.GetListAnswer(question.ID);
                return Success(new { question, listAnswer });
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult CreateQuestion(BusinessAccountQuestionUpdateModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Question)) throw new Exception("Question cannot be left blank");
                if(model.ListAnswer.Count <= 0) throw new Exception("Answer cannot be left blank");
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);

                        BusinessAccountQuestionsService businessAccountQuestionsService = new BusinessAccountQuestionsService(connection);

                        C500 question = new C500();
                        question.ID = Guid.NewGuid().ToString();
                        question.CreatedBy = userAdmin.ID;
                        question.Question = model.Question;
                        question.Status = C500.EnumStatus.ENABLED;
                        question.Type = model.Type;
                        if(businessAccountQuestionsService.CheckBusinessAccountQuestionsExist(question,transaction)) throw new Exception("Question existed");
                        if (!businessAccountQuestionsService.InsertQuestion(question, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        for (int indexAnswer = 0; indexAnswer < model.ListAnswer.Count; indexAnswer++) {
                            if (string.IsNullOrEmpty(model.ListAnswer[indexAnswer].Answer)) throw new Exception("Answer cannot be left blank");
                            model.ListAnswer[indexAnswer].ID = Guid.NewGuid().ToString();
                            model.ListAnswer[indexAnswer].QuestionID = question.ID;
                            if (!businessAccountQuestionsService.InsertAnswer(model.ListAnswer[indexAnswer],transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }


                        LogProvider.InsertLog("Thêm câu hỏi", userAdmin, connection, transaction);
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
        public JsonResult UpdateCreateQuestion(BusinessAccountQuestionUpdateModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Question)) throw new Exception("Question cannot be left blank");
                if (model.ListAnswer.Count <= 0) throw new Exception("Answer cannot be left blank");
                using (var connection = BaseService.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request, connection, transaction);
                        BusinessAccountQuestionsService businessAccountQuestionsService = new BusinessAccountQuestionsService(connection);
                        C500 question=  businessAccountQuestionsService.GetQuestionById(model.ID,transaction);
                        if (question == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                        question.ModifiedBy = userAdmin.ID;
                        question.Question = model.Question;
                        question.Type = model.Type;
                        if (businessAccountQuestionsService.CheckBusinessAccountQuestionsExist(question, transaction)) throw new Exception("Question existed");
                        if (!businessAccountQuestionsService.UpdateQuestion(question, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        for (int indexAnswer = 0; indexAnswer < model.ListAnswer.Count; indexAnswer++)
                        {
                            if (string.IsNullOrEmpty(model.ListAnswer[indexAnswer].Answer)) throw new Exception("Answer cannot be left blank");
                            if (string.IsNullOrEmpty(model.ListAnswer[indexAnswer].ID))
                            {
                                model.ListAnswer[indexAnswer].ID = Guid.NewGuid().ToString();
                                model.ListAnswer[indexAnswer].QuestionID = question.ID;
                                if (!businessAccountQuestionsService.InsertAnswer(model.ListAnswer[indexAnswer],transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                            }
                        }

                        foreach (var item in model.ListAnswerDelete) {
                            C501 answer = businessAccountQuestionsService.GetAnswer(item, transaction);
                            if (answer == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);
                            if (!businessAccountQuestionsService.DeleteAnswer(answer.ID,transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
                        }


                        LogProvider.InsertLog("Cập nhật câu hỏi", userAdmin, connection, transaction);
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
        public JsonResult DeleteQuestion(string id) {
            try {
                using (var connection = BaseService.Connect()) {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) {
                        S200 userAdmin = HelperProvider.GetUserFromRequest(Request,connection,transaction);
                        BusinessAccountQuestionsService businessAccountQuestionsService = new BusinessAccountQuestionsService(connection);

                        C500 question = businessAccountQuestionsService.GetQuestionById(id,transaction);
                        if (question == null) throw new Exception(JsonResult.Message.DATA_NOT_FOUND);

                        question.Status = C500.EnumStatus.DISABLED;
                        question.ModifiedBy = userAdmin.ID;
                        if (!businessAccountQuestionsService.DeleteQuestion(question.ID, transaction)) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

                        LogProvider.InsertLog("Xóa câu hỏi",userAdmin,connection,transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
        }
    }
}
