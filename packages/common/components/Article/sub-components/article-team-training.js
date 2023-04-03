import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";

import AlertComponent from "../../AlertComponent";

const ArticleTeamTrainingAlert = ({
  theme,
  trainingEmail,
  organizationName,
}) => {
  const { t, ready } = useTranslation("Common");
  const [isClose, setIsClose] = useState(
    localStorage.getItem("teamTrainingAlertClose")
  );

  const onClick = () => {
    localStorage.setItem("teamTrainingAlertClose", true);
    setIsClose(true);
  };

  if (isClose) return <></>;

  const isShowLoader = !ready;

  return isShowLoader ? (
    <Loaders.Rectangle width="210px" height="88px" />
  ) : (
    <AlertComponent
      titleColor={theme.catalog.teamTrainingAlert.titleColor}
      linkColor={theme.catalog.teamTrainingAlert.linkColor}
      borderColor={theme.catalog.teamTrainingAlert.borderColor}
      title={t("Common:UseLikePro", { organizationName })}
      description={t("Common:BookTeamTraining")}
      link={`mailto:${trainingEmail}`}
      linkTitle={t("Common:BookNow")}
      onCloseClick={onClick}
      needCloseIcon
    />
  );
};

export default withRouter(
  inject(({ auth }) => {
    const { settingsStore } = auth;

    const { theme, additionalResourcesData, organizationName } = settingsStore;

    return {
      theme,
      trainingEmail: additionalResourcesData.trainingEmail,
      organizationName,
    };
  })(observer(ArticleTeamTrainingAlert))
);
