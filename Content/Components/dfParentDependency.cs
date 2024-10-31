using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Components
{
    public class dfParentDependency : MonoBehaviour
    {
        public GameObject dependsOn;

        public void Update()
        {
            if (dependsOn == null)
                Destroy(gameObject);
        }
    }
}
