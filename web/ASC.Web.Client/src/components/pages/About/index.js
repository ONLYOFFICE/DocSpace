import React, { useEffect } from "react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import PageLayout from "@appserver/common/components/PageLayout";
import { I18nextProvider, Trans, withTranslation } from "react-i18next";
import styled from "styled-components";
import { isMobileOnly } from "react-device-detect";
import { setDocumentTitle } from "../../../helpers/utils";
import i18n from "./i18n";
import withLoader from "../Confirm/withLoader";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";

const BodyStyle = styled.div`
  width: 100%;
  padding: ${isMobileOnly ? "48px 0 0" : "80px 147px 0"};

  .avatar {
    margin-top: 32px;
    margin-bottom: 16px;
  }

  .row {
    display: flex;
    flex-direction: row;
  }

  .copyright {
    margin-top: 16px;
  }
`;

const Body = ({ t, personal, versionAppServer }) => {
  useEffect(() => {
    setDocumentTitle(t("Common:About"));
  }, [t]);

  const versionEditor = "6.3.1";
  const license = "AGPL-3.0";
  const link = "https://github.com/ONLYOFFICE";
  const phone = "+371 660-16425";
  const email = "support@onlyoffice.com";
  const address =
    "20A-12 Ernesta Birznieka-Upisha street, Riga, Latvia, EU, LV-1050";

  return (
    <BodyStyle>
      <Text fontSize="32px" fontWeight="600">
        {t("AboutHeader")}
      </Text>

      <div className="avatar">
        {personal ? (
          <ReactSVG src="images/logo_personal_about.svg" />
        ) : (
          <img
            className="logo-img"
            src="images/dark_general.png"
            width="320"
            height="181"
            alt="Logo"
          />
        )}
      </div>

      <div className="row">
        <Text fontSize="13px">{t("DocumentManagement")}:</Text>
        <Link
          color="#2DA7DB"
          fontSize="13px"
          fontWeight="600"
          href={link}
          target="_blank"
        >
          ONLYOFFICE App Server
        </Link>
        <Text fontSize="13px" fontWeight="600">
          v.{versionAppServer}
        </Text>
      </div>

      <div className="row">
        <Text fontSize="13px">{t("OnlineEditors")}:</Text>
        <Link
          color="#2DA7DB"
          fontSize="13px"
          fontWeight="600"
          href={link}
          target="_blank"
        >
          ONLYOFFICE Docs
        </Link>
        <Text fontSize="13px" fontWeight="600">
          v.{versionEditor}
        </Text>
      </div>

      <div className="row">
        <Text fontSize="13px">{t("SoftwareLicense")}: </Text>
        <Text fontSize="13px" fontWeight="600">
          {license}
        </Text>
      </div>

      <Text className="copyright" fontSize="14px" fontWeight="600">
        © Ascensio System SIA
      </Text>

      <div className="row">
        <Text fontSize="13px">{t("AboutCompanyAddressTitle")}:</Text>
        <Text fontSize="13px">{address}</Text>
      </div>

      <div className="row">
        <Text fontSize="13px">{t("AboutCompanyTelTitle")}:</Text>
        <Text fontSize="13px">{phone}</Text>
      </div>
      <div className="row">
        <Text fontSize="13px">{t("AboutCompanyEmailTitle")}:</Text>
        <Link
          color="#2DA7DB"
          fontSize="13px"
          fontWeight="600"
          href={`mailto:${email}`}
        >
          {email}
        </Link>
      </div>
    </BodyStyle>
  );
};

const BodyWrapper = inject(({ auth }) => ({
  personal: auth.settingsStore,
  versionAppServer: auth.settingsStore.version,
}))(withTranslation(["About", "Common"])(withLoader(observer(Body))));

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
