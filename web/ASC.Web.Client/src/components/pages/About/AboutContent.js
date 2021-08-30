import React from "react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import NoUserSelect from "@appserver/components/utils/commonStyles";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { isDesktop } from "react-device-detect";

const StyledAboutBody = styled.div`
  width: 100%;

  .avatar {
    margin-top: ${isDesktop ? "0px" : "32px"};
    margin-bottom: 16px;
  }

  .row-el {
    display: inline-block;
  }

  .copyright {
    margin-top: 16px;
  }
  .no-select {
    ${NoUserSelect}
  }
`;

const AboutContent = ({ personal, versionAppServer }) => {
  const { t } = useTranslation("About");
  const versionEditor = "6.3.1";
  const license = "AGPL-3.0";
  const linkAppServer = "https://github.com/ONLYOFFICE/AppServer";
  const linkDocs = "https://github.com/ONLYOFFICE/DocumentServer";
  const phone = "+371 660-16425";
  const email = "support@onlyoffice.com";
  const address =
    "20A-12 Ernesta Birznieka-Upisha street, Riga, Latvia, EU, LV-1050";

  return (
    <StyledAboutBody>
      <div className="avatar">
        {personal ? (
          <ReactSVG
            src="/images/logo_personal_about.svg"
            className="no-select"
          />
        ) : (
          <img
            src="/images/dark_general.png"
            alt="Logo"
            className="no-select"
          />
        )}
      </div>

      <div className="row">
        <Text className="row-el" fontSize="13px" noSelect>
          {t("DocumentManagement")}:
        </Text>
        <Link
          className="row-el"
          color="#2DA7DB"
          fontSize="13px"
          fontWeight="600"
          href={linkAppServer}
          target="_blank"
        >
          &nbsp;ONLYOFFICE App Server&nbsp;
        </Link>
        <Text className="row-el" fontSize="13px" fontWeight="600" noSelect>
          v.{versionAppServer}
        </Text>
      </div>

      <div className="row">
        <Text className="row-el" fontSize="13px" noSelect>
          {t("OnlineEditors")}:
        </Text>
        <Link
          className="row-el"
          color="#2DA7DB"
          fontSize="13px"
          fontWeight="600"
          href={linkDocs}
          target="_blank"
        >
          &nbsp;ONLYOFFICE Docs&nbsp;
        </Link>
        <Text className="row-el" fontSize="13px" fontWeight="600" noSelect>
          v.{versionEditor}
        </Text>
      </div>

      <div className="row">
        <Text className="row-el" fontSize="13px" noSelect>
          {t("SoftwareLicense")}:{" "}
        </Text>
        <Text className="row-el" fontSize="13px" fontWeight="600" noSelect>
          &nbsp;{license}
        </Text>
      </div>

      <Text className="copyright" fontSize="14px" fontWeight="600" noSelect>
        © Ascensio System SIA
      </Text>

      <div className="row">
        <Text fontSize="13px" noSelect>
          {t("AboutCompanyAddressTitle")}: {address}
        </Text>
      </div>

      <div className="row">
        <Text fontSize="13px" noSelect>
          {t("AboutCompanyTelTitle")}: {phone}
        </Text>
      </div>

      <div className="row">
        <Text className="row-el" fontSize="13px" noSelect>
          {t("AboutCompanyEmailTitle")}:
        </Text>
        <Link
          className="row-el"
          color="#2DA7DB"
          fontSize="13px"
          fontWeight="600"
          href={`mailto:${email}`}
        >
          &nbsp;{email}
        </Link>
      </div>
    </StyledAboutBody>
  );
};

export default AboutContent;
