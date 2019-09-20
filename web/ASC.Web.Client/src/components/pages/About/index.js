import React from "react";
import { PageLayout, Text, Link } from "asc-web-components";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import version from "../../../../package.json";
import styled from "styled-components";

const BodyStyle = styled.div`
  margin-top: 24px;
  .text_p {
    text-align: center;
  }
  .text_span {
    text-align: center;
  }

  .logo-img {
    text-align: center;
    max-width: 216px;
    max-height: 35px;
  }

  .copyright-line {
    padding-bottom: 15px;
    text-align: center;

    :before {
      background-color: #e1e1e1;
      content: "";
      height: 2px;
      margin-top: 9px;
      width: 36%;
      float: right;
    }

    :after {
      background-color: #e1e1e1;
      content: "";
      height: 2px;
      margin-top: 9px;
      width: 36%;
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

const Body = () => {
  const { t } = useTranslation("translation", { i18n });

  return (
    <BodyStyle>
      <p style={{ textAlign: "center", margin: "0px" }}>
        <img
          className="logo-img"
          src="images/dark_general.png"
          width="320"
          height="181"
          alt="Logo"
        ></img>
      </p>

      <VersionStyle>
        <Text.Body className="text_p" fontSize={14} color="#A3A9AE">
          {`${t("AboutCompanyVersion")}: ${version.version}`}
        </Text.Body>
      </VersionStyle>

      <Text.Body className="copyright-line" fontSize={14}>
        {t("AboutCompanyLicensor")}
      </Text.Body>

      <Text.Body className="text_p" fontSize={16} isBold={true}>
        Ascensio System SIA
      </Text.Body>

      <Style>
        <Text.Body className="text_p" fontSize={12}>
          <Text.Body
            className="text_span"
            fontSize={12}
            as="span"
            color="#A3A9AE"
          >
            {t("AboutCompanyAddressTitle")}:{" "}
          </Text.Body>
          20A-12 Ernesta Birznieka-Upisha street, Riga, Latvia, EU, LV-1050
        </Text.Body>

        <Text.Body
          fontSize={12}
          className="text_span"
          as="span"
          color="#A3A9AE"
        >
          {t("AboutCompanyEmailTitle")}:{" "}
          <Link href="mailto:support@onlyoffice.com" fontSize={12}>
            support@onlyoffice.com
          </Link>
        </Text.Body>

        <div style={{ marginTop: "4px" }}>
          <Text.Body className="text_p" fontSize={12}>
            <Text.Body
              fontSize={12}
              className="text_span"
              as="span"
              color="#A3A9AE"
            >
              {t("AboutCompanyTelTitle")}:{" "}
            </Text.Body>
            +371 660-16425
          </Text.Body>
        </div>

        <Link href="http://www.onlyoffice.com" fontSize={12}>
          www.onlyoffice.com
        </Link>

        <div style={{ marginTop: "20px" }}>
          <Text.Body className="text_p" fontSize={12}>
            {t("LicensedUnder")}:{" "}
            <Link
              href="https://www.gnu.org/licenses/gpl-3.0.html"
              isHovered={true}
              fontSize={12}
            >
              GNU GPL v.3
            </Link>{" "}
          </Text.Body>

          <Text.Body className="text_p" fontSize={12}>
            {t("SourceCode")}:{" "}
            <Link
              href="https://github.com/ONLYOFFICE/CommunityServer"
              isHovered={true}
              fontSize={12}
            >
              GitHub
            </Link>
          </Text.Body>
        </div>
      </Style>
    </BodyStyle>
  );
};

const About = () => {
  return <PageLayout sectionBodyContent={<Body />} />;
};

export default About;
