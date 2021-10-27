import React, { useEffect } from "react";
import Text from "@appserver/components/text";
import PageLayout from "@appserver/common/components/PageLayout";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import { isMobileOnly } from "react-device-detect";
import { setDocumentTitle } from "../../../helpers/utils";
import i18n from "./i18n";
import withLoader from "../Confirm/withLoader";
import { inject, observer } from "mobx-react";
import AboutContent from "./AboutContent";

const BodyStyle = styled.div`
  padding: ${isMobileOnly ? "48px 0 0" : "80px 147px 0"};
`;

const Body = ({ t, personal, buildVersionInfo }) => {
  useEffect(() => {
    setDocumentTitle(t("Common:About"));
  }, [t]);

  return (
    <BodyStyle>
      <Text fontSize="32px" fontWeight="600">
        {t("AboutHeader")}
      </Text>

      <AboutContent personal={personal} buildVersionInfo={buildVersionInfo} />
    </BodyStyle>
  );
};

const BodyWrapper = inject(({ auth }) => {
  const { personal, buildVersionInfo } = auth.settingsStore;
  return {
    personal,
    buildVersionInfo,
  };
})(withTranslation(["About", "Common"])(withLoader(observer(Body))));

const About = (props) => {
  return (
    <I18nextProvider i18n={i18n}>
      <PageLayout>
        <PageLayout.SectionBody>
          <BodyWrapper {...props} />
        </PageLayout.SectionBody>
      </PageLayout>
    </I18nextProvider>
  );
};

export default About;
