# OICNet.Server

Part of wanting to make OIC as easily adoptable by existing .Net developers. This project aims to provide an API that closely matches Microsoft's ASP.Net.

Because ASP.Net focuses heavily on an HTTP context, it was too big of a task to fork the library and "replace" HTTP with OIC and any accompanying data models. Instead, a minimal clone was made by copying over classes that were needed to provide an application layer, manage logging and dependency injection and providing a startup consumer for getting started.

