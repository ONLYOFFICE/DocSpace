import React from "react";
import Submenu from "@appserver/components/submenu";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import config from "../../../../../../package.json";

import AccessRights from "./access-rights";
import AccessPortal from "./access-portal/index.js";

const SecurityWrapper = (props) => {
  const { t, history } = props;

  const data = [
    {
      id: "access-rights",
      name: t("AccessRights"),
      content: <AccessPortal />,
    },
    {
      id: "access-portal",
      name: t("PortalAccess"),
      content: <AccessRights />,
    },
  ];

  const onSelect = (e) => {
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/settings/security/${e.id}`
      )
    );
  };

  return <Submenu data={data} startSelect={0} onSelect={(e) => onSelect(e)} />;
};

export default withTranslation("Settings")(withRouter(SecurityWrapper));
