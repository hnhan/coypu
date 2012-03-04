using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Coypu.Drivers.Selenium
{
    internal class ElementFinder
    {
        private readonly XPath xPath;
        private readonly RemoteWebDriver selenium;
        private string _outerWindowHandle;

        public ElementFinder(XPath xPath, RemoteWebDriver selenium)
        {
            this.xPath = xPath;
            this.selenium = selenium;
        }

        public IEnumerable<IWebElement> FindByPartialId(string id, DriverScope scope)
        {
            var xpath = String.Format(".//*[substring(@id, string-length(@id) - {0} + 1, string-length(@id)) = {1}]",
                                      id.Length, xPath.Literal(id));
            return Find(By.XPath(xpath),scope);
        }

        public IEnumerable<IWebElement> Find(By by, DriverScope scope)
        {
            return SeleniumScope(scope).FindElements(by).Where(e => IsDisplayed(e, scope));
        }

        public ISearchContext SeleniumScope(DriverScope scope)
        {
            var context = (ISearchContext) scope.Now().Native;
            if (context == selenium && _outerWindowHandle != null)
                selenium.SwitchTo().Window(_outerWindowHandle);

            _outerWindowHandle = selenium.CurrentWindowHandle;
            var frame = context as IWebElement;
            if (frame != null && frame.TagName == "iframe")
            {
                selenium.SwitchTo().Frame(frame);
                context = selenium;
            }

            return context;
        }

        public bool IsDisplayed(IWebElement e, DriverScope scope)
        {
            return scope.ConsiderInvisibleElements || e.IsDisplayed();
        }
    }
}