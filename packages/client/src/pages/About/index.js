import React, { useEffect } from "react";
import Section from "@docspace/common/components/Section";
import { I18nextProvider, withTranslation } from "react-i18next";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import i18n from "./i18n";
import withLoader from "../Confirm/withLoader";
import { inject, observer } from "mobx-react";
import AboutHeader from "./AboutHeader";
import AboutContent from "./AboutContent";

const Body = ({ t, personal, buildVersionInfo, theme }) => {
  useEffect(() => {
    setDocumentTitle(t("Common:About"));
  }, [t]);

  return (
    <AboutContent
      theme={theme}
      personal={personal}
      buildVersionInfo={buildVersionInfo}
    />
  );
};

const BodyWrapper = inject(({ auth }) => {
  const { personal, buildVersionInfo, theme } = auth.settingsStore;
  return {
    personal,
    buildVersionInfo,
    theme,
  };
})(withTranslation(["About", "Common"])(withLoader(observer(Body))));

const About = (props) => {
  return (
    <I18nextProvider i18n={i18n}>
      <Section>
        <Section.SectionHeader>
          <AboutHeader />
        </Section.SectionHeader>
        <Section.SectionBody>
          <BodyWrapper {...props} />
        </Section.SectionBody>
      </Section>
    </I18nextProvider>
  );
};

export default About;
