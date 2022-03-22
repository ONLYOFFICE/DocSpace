import React from "react";
import styled from "styled-components";
import { Trans } from "react-i18next";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";

const StyledTooltip = styled.div`
  .font-size {
    font-size: 12px;
  }
  .bold {
    font-weight: 600;
  }
  .display {
    display: inline;
  }
  .display-link {
    display: block;
  }
`;

export const LanguageTimeSettingsTooltip = ({ t, theme }) => {
  const learnMore = t("Common:LearnMore");
  const text = t("Settings:StudioTimeLanguageSettings");
  const save = t("Common:SaveButton");

  return (
    <StyledTooltip>
      <Text className="font-size">
        <Trans ns="Settings" i18nKey="LanguageTimeSettingsTooltip" text={text}>
          <Text className="bold display font-size">{{ text }}</Text>{" "}
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
          <Text className="bold display font-size"> {{ save }}</Text>{" "}
          <Link
            color={theme.studio.settings.common.linkColorHelp}
            className="display-link font-size"
            isHovered={true}
            href="https://helpcenter.onlyoffice.com/administration/configuration.aspx#CustomizingPortal_block"
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
          <Text className="bold display font-size"> {{ welcomeText }}</Text>{" "}
          <Text className="bold display font-size"> {{ text }}</Text>{" "}
          <Text className="bold display font-size"> {{ from }}</Text>{" "}
        </Trans>
      </Text>
      <Text className="font-size">
        <Trans
          ns="Settings"
          i18nKey="CustomTitlesSettingsTooltipDescription"
          header={header}
        >
          {" "}
          <Text className="bold display font-size"> {{ header }}</Text>{" "}
        </Trans>
      </Text>
    </StyledTooltip>
  );
};
