import PortalSettingsRouter from "./portalSettings";
import ClientRouter from "./client";
import ConfirmRouter from "./confirm";

const routes = [...ClientRouter, PortalSettingsRouter, ...ConfirmRouter];

export default routes;
