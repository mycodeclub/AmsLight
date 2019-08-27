using AmsLight.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AmsLight.Controllers
{
    [Authorize]

    public class SuperAdminController : Controller
    {
        private AmsDbContext db = new AmsDbContext();
        public SuperAdminController() { }
        // GET: SuperAdmin
        public ActionResult Index()
        {
            return View(db.TrainingPartner.ToList());
        }

        public ActionResult Create()
        {
            return View(new TrainingPartner() { Login = new Login() { } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingPartner tp)
        {
            if (ModelState.IsValid)
            {
                tp.RegistrationDate = System.DateTime.Now;
                tp.SubscriptionStartDate = System.DateTime.Now;
                tp.SubscriptionEndDate = System.DateTime.Now.AddDays(360);
                tp.IsActive = false;
                db.TrainingPartner.Add(tp);
                db.SaveChanges();
                db.Logins.Add(new Login()
                {
                    TpId = tp.TpId,
                    UserName = tp.Login.UserName,
                    Password = tp.Login.Password,
                    TypeId = 2,
                    CreatedDate = System.DateTime.Now
                }) ;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tp);
        }

        // GET: TrainingPartners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingPartner trainingPartner = db.TrainingPartner.Find(id);
            trainingPartner.Login = db.Logins.Where(l => l.TpId == id).FirstOrDefault();
            if (trainingPartner == null)
            {
                return HttpNotFound();
            }
            return View(trainingPartner);
        }

        // POST: TrainingPartners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TrainingPartner tp)
        {
            if (ModelState.IsValid)
            {
                var _tp = db.TrainingPartner.Find(tp.TpId);
                tp.RegistrationDate = _tp.RegistrationDate;
                tp.SubscriptionStartDate = _tp.SubscriptionStartDate;
                tp.SubscriptionEndDate = _tp.SubscriptionEndDate;
                db.Entry(tp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tp);
        }


        // GET: TrainingPartners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingPartner trainingPartner = db.TrainingPartner.Find(id);
            trainingPartner.Login = db.Logins.Where(l => l.TpId == id).FirstOrDefault();
            if (trainingPartner == null)
            {
                return HttpNotFound();
            }
            return View(trainingPartner);
        }

    }
}