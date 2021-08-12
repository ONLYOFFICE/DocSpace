﻿import React, { useEffect } from "react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import PageLayout from "@appserver/common/components/PageLayout";
import { I18nextProvider, Trans, withTranslation } from "react-i18next";
import styled from "styled-components";
import { isMobile } from "react-device-detect";
import { setDocumentTitle } from "../../../helpers/utils";
import i18n from "./i18n";
import withLoader from "../Confirm/withLoader";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";

const BodyStyle = styled.div`
  margin-top: ${isMobile ? "80px" : "24px"};

  .avatar {
    text-align: center;
    margin: 0px;
  }

  .text-about {
    margin-top: 4px;
  }

  .text-license {
    margin-top: 20px;
  }

  .text_style {
    text-align: center;
  }

  .logo-img {
    text-align: center;
    max-width: 216px;
    max-height: 35px;
  }

  .hidden-text {
    height: 0;
    visibility: hidden;
    margin: 0;
  }

  .copyright-line {
    display: grid;
    grid-template-columns: 1fr max-content 1fr;
    grid-column-gap: 24px;
    padding-bottom: 15px;
    text-align: center;

    :before {
      background-color: #e1e1e1;
      content: "";
      height: 2px;
      margin-top: 9px;
      float: right;
    }

    :after {
      background-color: #e1e1e1;
      content: "";
      height: 2px;
      margin-top: 9px;
      float: left;
    }
  }
`;

const Style = styled.div`
  margin-top: 8px;
  text-align: center;
`;

const VersionStyle = styled.div`
  padding: 8px 0px 20px 0px;
`;

const Body = ({ t, personal, version }) => {
  useEffect(() => {
    setDocumentTitle(t("Common:About"));
  }, [t]);

  const gitHub = "GitHub";
  const license = "AGPL-3.0";
  const link = "www.onlyoffice.com";
  const phone = "+371 660-16425";
  const supportLink = "support@onlyoffice.com";
  const address =
    "20A-12 Ernesta Birznieka-Upisha street, Riga, Latvia, EU, LV-1050";
  const licenseContent = (
    <Text as="div" className="text_style" fontSize="12px">
      <Trans t={t} i18nKey="LicensedUnder" ns="About">
        "This software is licensed under:"
        <Link
          href="https://www.gnu.org/licenses/gpl-3.0.html"
          isHovered={true}
          fontSize="12px"
          target="_blank"
        >
          {{ license }}
        </Link>
      </Trans>
    </Text>
  );

  return (
    <BodyStyle>
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

      <VersionStyle>
        <Text className="text_style" fontSize="14px" color="#A3A9AE">
          {`${t("Common:Version")}: ${version}`}
        </Text>
      </VersionStyle>

      <Text className="copyright-line" fontSize="14px">
        {t("AboutCompanyLicensor")}
      </Text>

      <Text as="div" className="text_style" fontSize="16px" isBold={true}>
        <Trans t={t} i18nKey="AllRightsReservedCustomMode" ns="About">
          Ascensio System SIA
          <p className="hidden-text">All rights reserved.</p>
        </Trans>
      </Text>

      <Style>
        <Text className="text_style" fontSize="12px">
          <Text
            className="text_style"
            fontSize="12px"
            as="span"
            color="#A3A9AE"
          >
            {t("AboutCompanyAddressTitle")}:{" "}
          </Text>
          {address}
        </Text>

        <Text fontSize="12px" className="text_style" as="span" color="#A3A9AE">
          {t("AboutCompanyEmailTitle")}:{" "}
          <Link href="mailto:support@onlyoffice.com" fontSize="12px">
            {supportLink}
          </Link>
        </Text>

        <div className="text-about">
          <Text className="text_style" fontSize="12px">
            <Text
              fontSize="12px"
              className="text_style"
              as="span"
              color="#A3A9AE"
            >
              {t("AboutCompanyTelTitle")}:{" "}
            </Text>
            {phone}
          </Text>
        </div>
        <Link href="http://www.onlyoffice.com" fontSize="12px" target="_blank">
          {link}
        </Link>

        <div className="text-license">
          <div className="text-row">{licenseContent}</div>

          <Text className="text_style" fontSize="12px">
            {t("SourceCode")}:{" "}
            <Link
              href="https://github.com/ONLYOFFICE/AppServer"
              isHovered={true}
              fontSize="12px"
              target="_blank"
            >
              {gitHub}
            </Link>
          </Text>
        </div>
      </Style>
    </BodyStyle>
  );
};

const BodyWrapper = inject(({ auth }) => ({
  personal: auth.settingsStore,
  version: auth.settingsStore.version,
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
