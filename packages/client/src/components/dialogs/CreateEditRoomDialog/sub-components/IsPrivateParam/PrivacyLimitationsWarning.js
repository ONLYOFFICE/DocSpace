import React from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";

import { Base } from "@docspace/components/themes";

const StyledPrivacyLimitationsWarning = styled.div`
  box-sizing: border-box;
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 4px;
  background: ${(props) =>
    props.theme.createEditRoomDialog.isPrivate.limitations.background};
  border-radius: 6px;
  padding: 12px 8px;

  .warning-title {
    display: flex;
    flex-direction: row;
    gap: 8px;
    &-icon-wrapper {
      display: flex;
      align-items: center;
      .warning-title-icon {
        width: 16px;
        height: 14px;
        path {
          fill: ${(props) =>
            props.theme.createEditRoomDialog.isPrivate.limitations.iconColor};
        }
      }
    }

    &-text {
      font-weight: 600;
      font-size: 13px;
      line-height: 20px;
      color: ${(props) =>
        props.theme.createEditRoomDialog.isPrivate.limitations.titleColor};
    }
  }

  .warning-description {
    font-weight: 400;
    font-size: 12px;
    line-height: 16px;
    color: ${(props) =>
      props.theme.createEditRoomDialog.isPrivate.limitations.descriptionColor};
  }

  .warning-link {
    cursor: pointer;
    margin-top: 2px;
    font-weight: 600;
    font-size: 13px;
    line-height: 15px;
    color: ${(props) =>
      props.theme.createEditRoomDialog.isPrivate.limitations.linkColor};
    text-decoration: underline;
    text-underline-offset: 1px;
  }
`;

StyledPrivacyLimitationsWarning.defaultProps = { theme: Base };

const PrivacyLimitationsWarning = ({ t }) => {
  return (
    <StyledPrivacyLimitationsWarning>
      <div className="warning-title">
        <div className="warning-title-icon-wrapper">
          <ReactSVG
            className="warning-title-icon"
            src={"/static/images/danger.alert.react.svg"}
          />
        </div>
        <div className="warning-title-text">{t("Common:Warning")}</div>
      </div>
      <div className="warning-description">
        {/* {t("MakeRoomPrivateLimitationsWarningDescription")} */}
      </div>
      <div className="warning-link">{t("Common:LearnMore")}</div>
    </StyledPrivacyLimitationsWarning>
  );
};

export default PrivacyLimitationsWarning;
