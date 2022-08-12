import React from "react";
import styled from "styled-components";
import { Trans } from "react-i18next";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";

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
      <Text className="font-size">
        <Trans ns="Settings" i18nKey="LanguageTimeSettingsTooltip" text={text}>
          <Text className="bold display-inline font-size">{{ text }}</Text>{" "}
        </Trans>
      </Text>
      <Text className="font-size">
        <Trans
          ns="Settings"
          i18nKey="LanguageTimeSettingsTooltipDescription"
          learnMore={learnMore}
          save={save}
        >
          {" "}
          <Text className="bold display-inline font-size">
            {" "}
            {{ save }}
          </Text>{" "}
          <Link
            color={theme.client.settings.common.linkColorHelp}
            className="display-block font-size"
            isHovered={true}
            href={`${helpLink}/administration/configuration.aspx#CustomizingPortal_block`}
          >
            {{ learnMore }}
          </Link>
        </Trans>
      </Text>
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
      <Text className="font-size">
        <Trans
          ns="Settings"
          i18nKey="CustomTitlesSettingsTooltip"
          welcomeText={welcomeText}
          text={text}
          from={from}
        >
          <Text className="bold display-inline font-size">
            {" "}
            {{ welcomeText }}
          </Text>{" "}
          <Text className="bold display-inline font-size"> {{ text }}</Text>{" "}
          <Text className="bold display-inline font-size"> {{ from }}</Text>{" "}
        </Trans>
      </Text>
      <Text className="font-size">
        <Trans
          ns="Settings"
          i18nKey="CustomTitlesSettingsTooltipDescription"
          header={header}
        >
          {" "}
          <Text className="bold display-inline font-size">
            {" "}
            {{ header }}
          </Text>{" "}
        </Trans>
      </Text>
    </StyledTooltip>
  );
};

export const PortalRenamingTooltip = ({ t }) => {
  const text = t("Settings:PortalRenamingDescription");
  const pleaseNote = t("Settings:PleaseNote");
  const save = t("Common:SaveButton");

  return (
    <StyledTooltip>
      <Text className="font-size">
        <Trans
          ns="Settings"
          i18nKey="PortalRenamingSettingsTooltip"
          text={text}
        >
          <Text className="display-inline font-size"> {{ text }}</Text>{" "}
        </Trans>
      </Text>
      <Text className="font-size">
        <Trans
          ns="Settings"
          i18nKey="PleaseNoteDescription"
          pleaseNote={pleaseNote}
          save={save}
        >
          <Text className="bold display-inline font-size">
            {" "}
            {{ pleaseNote }}
          </Text>{" "}
          <Text className="bold display-inline font-size"> {{ save }}</Text>{" "}
        </Trans>
      </Text>
    </StyledTooltip>
  );
};
