import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import { useTranslation } from "react-i18next";

import AlertComponent from "../../AlertComponent";
import Loaders from "../../Loaders";

const ArticleTeamTrainingAlert = ({ theme, bookTrainingEmail }) => {
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
      title={t("Common:UseLikePro")}
      description={t("Common:BookTeamTraining")}
      link={`mailto:${bookTrainingEmail}`}
      linkTitle={t("Common:BookNow")}
      onCloseClick={onClick}
      needCloseIcon
    />
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;

  const { theme, bookTrainingEmail } = settingsStore;

  return {
    theme,
    bookTrainingEmail,
  };
})(observer(ArticleTeamTrainingAlert));
