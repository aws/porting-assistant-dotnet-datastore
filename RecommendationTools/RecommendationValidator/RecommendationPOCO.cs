
using System.ComponentModel.DataAnnotations;

namespace RecommendationValidator
{
    public class RecommendationPOCO
    {
        /*
         * The namespace this file applies to
         */
        [Required]
        public string Name { get; set; }
        /*
         * Porting Assistant version, default 1.0
         */
        [Required]
        public string Version { get; set; }
        /*
         * The package that this namespace belongs to
         */
        [Required]
        public Packages[] Packages { get; set; }
        /*
         * The list of recommendations that are applicable under this namespace
         */
        [Required]
        public Recommendations[] Recommendations { get; set; }
    }

    public class Packages
    {
        /*
         * package name (sdk assembly name/nuget package name)
         */
        public string Name { get; set; }
        /*
         * sdk/nuget/other
         */
        public string Type { get; set; }
    }

    public class Recommendations
    {
        /*
         * The node to look for in the code for this recommendation. 
         * This can be  one of the following values:Namespace, Class, Attribute, Method
         * */
        [Required]
        public string Type { get; set; }
        /*
         * The name of the node to look for. 
         * For example, if we’re looking for a class, name can be the name of the class.
         */
        public string Name { get; set; }
        /*
         * The value of the node to look for. 
         * This is usually the name, in addition to the namespace and type. 
         * For example, the value of SelectList would be System.Web.Mvc.SelectList
         */
        [Required]
        public string Value { get; set; }
        /*
         * This is the type of key we’re looking for. By default, this looks for  the name of the node. However, there are other options to find nodes by. 
         * Below is a list of these options:
         * Namespace: Name
         * Class: BaseClass, ClassName, 
         * IdentifierAttribute: Name
         * Method: Name
         */
        public string KeyType { get; set; }
        [Required]
        public RecommendedActions[] RecommendedActions { get; set; }
    }

    public class RecommendedActions
    {
        /*
         * The source of the recommendation. 
         * This can be from the open source community  (OpenSource) or Amazon (Amazon)
         */
        [Required]
        public string Source { get; set; }
        /*
         * Yes/No
         */
        [Required]
        public string Preferred { get; set; }

        /*
         * This section describes what version of .net core this recommendation applies to. 
         * Some recommendations might be applicable to .net core 3.1, but  not 2.0.
         */
        [Required]
        public string[] TargetFrameworks { get; set; }
        /*
         * This is the description of the recommendation.
         */
        [Required]
        public string Description { get; set; }
        /*
         * optional auto apply actions
         */
        public Actions[] Actions { get; set; }
    }

    public class Actions
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
