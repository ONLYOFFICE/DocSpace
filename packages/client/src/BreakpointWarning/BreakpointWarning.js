import React from "react";

import { withTranslation } from "react-i18next";

import StyledBreakpointWarning from "./StyledBreakpointWarning";

const BreakpointWarning = ({ t, content }) => {
  return (
    <StyledBreakpointWarning>
      <img src="/static/images/breakpoint-warning.svg" />

      <div className="description">
        <div className="text-breakpoint">{t("BreakpointWarningText")}</div>
        <div className="text-prompt">
          {t("BreakpointWarningTextPrompt")} {content}
        </div>
      </div>
    </StyledBreakpointWarning>
  );
};

export default withTranslation(["Settings"])(BreakpointWarning);
