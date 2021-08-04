import React from "react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import { ReactSVG } from "react-svg";

const AboutBody = styled.div`
  width: 100%;

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

const AboutContent = ({ personal, versionAppServer }) => {
  const { t } = useTranslation("About");
  const versionEditor = "6.3.1";
  const license = "AGPL-3.0";
  const link = "https://github.com/ONLYOFFICE";
  const phone = "+371 660-16425";
  const email = "support@onlyoffice.com";
  const address =
    "20A-12 Ernesta Birznieka-Upisha street, Riga, Latvia, EU, LV-1050";

  return (
    <AboutBody>
      <div className="avatar">
        {personal ? (
          <ReactSVG src="images/logo_personal_about.svg" />
        ) : (
          <img src="images/dark_general.png" alt="Logo" />
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
        <Text fontSize="13px">
          {t("AboutCompanyAddressTitle")}: {address}
        </Text>
      </div>

      <div className="row">
        <Text fontSize="13px">
          {t("AboutCompanyTelTitle")}: {phone}
        </Text>
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
    </AboutBody>
  );
};

export default AboutContent;
