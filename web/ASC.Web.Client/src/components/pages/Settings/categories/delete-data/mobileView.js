import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { DeleteDataLayout } from "./StyledDeleteData";
import MobileCategoryWrapper from "../security/sub-components/mobile-category-wrapper";

const MobileView = (props) => {
  const { t, history } = props;

  const onClickLink = (e) => {
    e.preventDefault();
    history.push(e.target.pathname);
  };

  return (
    <DeleteDataLayout>
      <MobileCategoryWrapper
        title={t("PortalDeactivation")}
        subtitle={t("PortalDeactivationDescription")}
        url="/settings/datamanagement/delete-data/deactivation"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("PortalDeletion")}
        subtitle={t("PortalDeletionDescription")}
        url="/settings/datamanagement/delete-data/deletion"
        onClickLink={onClickLink}
      />
    </DeleteDataLayout>
  );
};

export default withTranslation("Settings")(withRouter(MobileView));
