import React, { Suspense, lazy } from "react";

import Loader from "@appserver/components/loader";
import { Switch } from "react-router";

const BackupSettings = lazy(() => import("./backup"));
const PROXY_BASE_URL = combineUrl(
  AppServerConfig.proxyURL,
  "/settings/datamanagement"
);

const BACKUP_URLS = [combineUrl(PROXY_BASE_URL, "/backup"), PROXY_BASE_URL];

const DataManagement = () => {
  return (
    <Suspense
      fallback={<Loader className="pageLoader" type="rombs" size="40px" />}
    >
      <Switch>
        <Route exact path={BACKUP_URLS} component={BackupSettings} />
      </Switch>
    </Suspense>
  );
};

export default DataManagement;
