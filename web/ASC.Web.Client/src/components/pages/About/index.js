import React, { useEffect } from "react";
import { connect } from "react-redux";
import { Text, Link } from "asc-web-components";
import { PageLayout } from "asc-web-common";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import version from "../../../../package.json";
import styled from "styled-components";
import { Trans } from "react-i18next";

const BodyStyle = styled.div`
  margin-top: 24px;

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
    padding-bottom: 15px;
    text-align: center;

    :before {
      background-color: #e1e1e1;
      content: "";
      height: 2px;
      margin-top: 9px;
      width: 26%;
      float: right;
    }

    :after {
      background-color: #e1e1e1;
      content: "";
      height: 2px;
      margin-top: 9px;
      width: 26%;
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

const Body = ({ language }) => {
  const { t } = useTranslation("translation", { i18n });

  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  const gitHub = "GitHub";
  const license = "AGPL-3.0";
  const link = "www.onlyoffice.com";
  const phone = "+371 660-16425";
  const supportLink = "support@onlyoffice.com";
  const address =
    "20A-12 Ernesta Birznieka-Upisha street, Riga, Latvia, EU, LV-1050";
  const licenseContent = (
    <Text as="div" className="text_style" fontSize="12px">
      <Trans i18nKey="LicensedUnder" i18n={i18n}>
        "This software is licensed under:"
        <Link
          href="https://www.gnu.org/licenses/gpl-3.0.html"
          isHovered={true}
          fontSize="12px"
        >
          {{ license }}
        </Link>
      </Trans>
    </Text>
  );

  return (
    <BodyStyle>
      <p className="avatar">
        <img
          className="logo-img"
          src="images/dark_general.png"
          width="320"
          height="181"
          alt="Logo"
        />
      </p>

      <VersionStyle>
        <Text className="text_style" fontSize="14px" color="#A3A9AE">
          {`${t("AboutCompanyVersion")}: ${version.version}`}
        </Text>
      </VersionStyle>

      <Text className="copyright-line" fontSize="14px">
        {t("AboutCompanyLicensor")}
      </Text>

      <Text as="div" className="text_style" fontSize="16px" isBold={true}>
        <Trans i18nKey="AllRightsReservedCustomMode" i18n={i18n}>
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
        <Link href="http://www.onlyoffice.com" fontSize="12px">
          {link}
        </Link>

        <div className="text-license">
          <div className="text-row">{licenseContent}</div>

          <Text className="text_style" fontSize="12px">
            {t("SourceCode")}:{" "}
            <Link
              href="https://github.com/ONLYOFFICE/CommunityServer"
              isHovered={true}
              fontSize="12px"
            >
              {gitHub}
            </Link>
          </Text>
        </div>
      </Style>
    </BodyStyle>
  );
};

const About = ({ language }) => (
  <PageLayout sectionBodyContent={<Body language={language} />} />
);

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName || state.auth.settings.culture
  };
}

export default connect(mapStateToProps)(About);
