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
    display: block;
  }
`;

export const LanguageTimeSettingsTooltip = ({ t, theme, helpLink }) => {
  const learnMore = t("Common:LearnMore");
  const text = t("Settings:StudioTimeLanguageSettings");
  const save = t("Common:SaveButton");

  return (
    <StyledTooltip>
      <div className="font-size">
        <Trans ns="Settings" i18nKey="LanguageTimeSettingsTooltip" text={text}>
          <div className="bold display-inline font-size">{{ text }}</div>{" "}
        </Trans>
      </div>
      <div className="font-size">
        <Trans
          ns="Settings"
          i18nKey="LanguageTimeSettingsTooltipDescription"
          learnMore={learnMore}
          save={save}
        >
          {" "}
          <div className="bold display-inline font-size"> {{ save }}</div>{" "}
          <Link
            color={theme.client.settings.common.linkColorHelp}
            className="display-block font-size"
            isHovered={true}
            target="_blank"
            href={`${helpLink}/administration/configuration.aspx#CustomizingPortal_block`}
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
          <div className="bold display-inline font-size">
            {" "}
            {{ welcomeText }}
          </div>{" "}
          <div className="bold display-inline font-size"> {{ text }}</div>{" "}
          <div className="bold display-inline font-size"> {{ from }}</div>{" "}
        </Trans>
      </div>
      <div className="font-size">
        <Trans
          ns="Settings"
          i18nKey="CustomTitlesSettingsTooltipDescription"
          header={header}
        >
          {" "}
          <div className="bold display-inline font-size">
            {" "}
            {{ header }}
          </div>{" "}
        </Trans>
      </div>
    </StyledTooltip>
  );
};

export const DNSSettingsTooltip = ({ t, theme, helpLink }) => {
  const text = t("Settings:DNSSettingsTooltip");
  const learnMore = t("Common:LearnMore");

  return (
    <StyledTooltip>
      <div className="font-size">
        <Trans
          ns="Settings"
          i18nKey="DNSSettingsTooltip"
          text={text}
          learnMore={learnMore}
        >
          <div className="display-inline font-size"> {{ text }}</div>{" "}
          <Link
            color={theme.client.settings.common.linkColorHelp}
            className="display-block font-size"
            isHovered={true}
            target="_blank"
            href={`${helpLink}/administration/configuration.aspx#CustomizingPortal_block`}
          >
            {{ learnMore }}
          </Link>
        </Trans>
      </div>
    </StyledTooltip>
  );
};

export const PortalRenamingTooltip = ({ t }) => {
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
        >
          <div className="display-inline font-size"> {{ text }}</div>{" "}
        </Trans>
      </div>
      <div className="font-size">
        <Trans
          ns="Settings"
          i18nKey="PleaseNoteDescription"
          pleaseNote={pleaseNote}
          save={save}
        >
          <div className="bold display-inline font-size"> {{ pleaseNote }}</div>{" "}
          <div className="bold display-inline font-size"> {{ save }}</div>{" "}
        </Trans>
      </div>
    </StyledTooltip>
  );
};
