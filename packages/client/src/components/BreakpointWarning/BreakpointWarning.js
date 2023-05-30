import BreakpointWarningSvgUrl from "PUBLIC_DIR/images/manage.access.rights.react.svg?url";
import BreakpointWarningSvgDarkUrl from "PUBLIC_DIR/images/manage.access.rights.dark.react.svg?url";
import React from "react";
import { Trans, withTranslation } from "react-i18next";
import StyledBreakpointWarning from "./sub-components/StyledBreakpointWarning";
import Loader from "./sub-components/loader";
import { inject, observer } from "mobx-react";

const BreakpointWarning = ({
  t,
  sectionName,
  tReady,
  theme,
  isSmallWindow,
}) => {
  const textHeader = isSmallWindow
    ? t("BreakpointSmallText")
    : t("BreakpointWarningText");

  const textPrompt = isSmallWindow ? (
    t("BreakpointSmallTextPrompt")
  ) : (
    <Trans t={t} i18nKey="BreakpointWarningTextPrompt" ns="Settings">
      "Please use the desktop site to access the {{ sectionName }}
      settings."
    </Trans>
  );

  const img = theme.isBase
    ? BreakpointWarningSvgUrl
    : BreakpointWarningSvgDarkUrl;

  return !tReady ? (
    <Loader />
  ) : (
    <StyledBreakpointWarning>
      <img src={img} />
      <div className="description">
        <div className="text-breakpoint">{textHeader}</div>
        <div className="text-prompt">{textPrompt}</div>
      </div>
    </StyledBreakpointWarning>
  );
};

export default inject(({ auth }) => {
  return {
    theme: auth.settingsStore.theme,
  };
})(observer(withTranslation(["Settings"])(BreakpointWarning)));
