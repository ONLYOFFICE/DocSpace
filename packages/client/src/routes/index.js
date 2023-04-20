import PortalSettingsRoutes from "./portalSettings";
import ClientRoutes from "./client";
import ConfirmRoutes from "./confirm";

const routes = [...ClientRoutes, PortalSettingsRoutes, ...ConfirmRoutes];

export default routes;
