import React from "react";
import styled from "styled-components";
import { Trans } from "react-i18next";
import Link from "@docspace/components/link";

const StyledTooltip = styled.div`
  .font-size {
    font-size: 12px;
  }
  .bold {
    font-weight: 600;
  }
  .display-inline {
    display: inline;
  }
  .display-block {
    margin-top: 10px;
    display: block;
    margin-top: 10px;
  }
`;

export const LanguageTimeSettingsTooltip = ({
  t,
  theme,
  helpLink,
  organizationName,
}) => {
  const learnMore = t("Common:LearnMore");
  const text = t("Settings:StudioTimeLanguageSettings");
  const save = t("Common:SaveButton");

  return (
    <StyledTooltip>
      <div className="font-size">
        <Trans ns="Settings" i18nKey="LanguageTimeSettingsTooltip" text={text}>
          <div className="bold display-inline font-size">{{ text }}</div>
          {{ organizationName }}
        </Trans>
      </div>
      <div className="font-size">
        <Trans
          ns="Settings"
          i18nKey="LanguageTimeSettingsTooltipDescription"
          learnMore={learnMore}
          save={save}
        >
          To make the parameters you set take effect click the
          <div className="bold display-inline font-size"> {{ save }}</div>
          button at the bottom of the section.
          <Link
            className="display-block"
            color="#333333"
            fontSize="13px"
            isHovered
            isBold
            target="_blank"
            href={`${helpLink}/administration/docspace-settings.aspx#DocSpacelanguage`}
          >
            {{ learnMore }}
          </Link>
        </Trans>
      </div>
    </StyledTooltip>
  );
};

export const CustomTitlesTooltip = ({ t }) => {
  const welcomeText = t("Settings:CustomTitlesWelcome");
  const text = t("Settings:CustomTitlesText");
  const from = t("Settings:CustomTitlesFrom");
  const header = t("Common:Title");
  return (
    <StyledTooltip>
      <div className="font-size">
        <Trans
          ns="Settings"
          i18nKey="CustomTitlesSettingsTooltip"
          welcomeText={welcomeText}
          text={text}
          from={from}
        >
          <div className="bold display-inline font-size">{{ welcomeText }}</div>
          is a way to change the default portal title to be displayed on the
          <div className="bold display-inline font-size"> {{ text }}</div>
          of your portal. The same name is also used for the
          <div className="bold display-inline font-size"> {{ from }}</div>
          field of your portal email notifications.
        </Trans>
      </div>
      <div className="font-size">
        <Trans
          ns="Settings"
          i18nKey="CustomTitlesSettingsTooltipDescription"
          header={header}
        >
          Enter the name you like in the
          <div className="bold display-inline font-size">{{ header }}</div>
          field.
        </Trans>
      </div>
    </StyledTooltip>
  );
};

export const DNSSettingsTooltip = ({
  t,
  helpLink,
  organizationName,
  standalone,
}) => {
  return (
    <StyledTooltip>
      <div className="font-size">
        {t("DNSSettingsTooltipMain", { organizationName })}{" "}
        {standalone
          ? t("DNSSettingsTooltipStandalone", { organizationName })
          : t("DNSSettingsMobile")}
        <Link
          color="#333333"
          className="display-block"
          fontSize="13px"
          isBold
          isHovered
          target="_blank"
          href={`${helpLink}/administration/docspace-settings.aspx#alternativeurl`}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>
    </StyledTooltip>
  );
};

export const PortalRenamingTooltip = ({ t, domain }) => {
  const text = t("Settings:PortalRenamingDescription");
  const pleaseNote = t("Settings:PleaseNote");
  const save = t("Common:SaveButton");

  return (
    <StyledTooltip>
      <div className="font-size">
        <Trans
          ns="Settings"
          i18nKey="PortalRenamingSettingsTooltip"
          text={text}
          domain={domain}
        >
          <div className="display-inline font-size"> {{ text }}</div>
          Enter the part that will appear next to the
          {{ domain }} portal address.
        </Trans>
      </div>
      <div className="font-size">
        <Trans
          ns="Settings"
          i18nKey="PleaseNoteDescription"
          pleaseNote={pleaseNote}
          save={save}
        >
          <div className="bold display-inline font-size"> {{ pleaseNote }}</div>
          : your old portal address will become available to new users once you
          click the
          <div className="bold display-inline font-size"> {{ save }}</div>
          button.
        </Trans>
      </div>
    </StyledTooltip>
  );
};
