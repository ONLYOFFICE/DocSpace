import React, { useEffect } from "react";
import styled from "styled-components";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import Button from "@appserver/components/button";
import PageLayout from "@appserver/common/components/PageLayout";
import { smallTablet, tablet } from "@appserver/components/utils/device";
import { I18nextProvider, Trans, withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { isMobile } from "react-device-detect";
//import { setDocumentTitle } from "../../helpers/utils";
import i18n from "./i18n";

const StyledPrivacyPage = styled.div`
  margin-top: ${isMobile ? "80px" : "36px"};

  .privacy-rooms-body {
    display: flex;
    flex-direction: column;
    align-items: center;
    max-width: 770px;
    margin: auto;
    margin-top: 80px;
  }

  .privacy-rooms-text-header {
    margin-bottom: 46px;
  }

  .privacy-rooms-text-dialog {
    margin-top: 32px;
    margin-bottom: 42px;
  }

  .privacy-rooms-text-separator {
    width: 70%;
    margin: 28px 0 42px 0;
    border-bottom: 1px solid #d3d3d3;
  }

  .privacy-rooms-install-text {
    text-align: left;

    @media ${smallTablet} {
      text-align: center;
    }
  }

  .privacy-rooms-install {
    display: flex;
    flex-direction: row;

    @media ${smallTablet} {
      flex-direction: column;
    }
  }

  .privacy-rooms-link {
    margin-left: 4px;
  }

  .privacy-rooms-text-description {
    margin-top: 28px;

    p {
      margin: 0;
    }
  }

  .privacy-rooms-avatar {
    text-align: left;
    padding-left: 66px;

    @media ${tablet} {
      padding-left: 74px;
    }

    @media ${smallTablet} {
      padding: 0px;
      text-align: center;
    }

    margin: 0px;
  }

  .privacy-rooms-logo {
    text-align: center;
    max-width: 216px;
    max-height: 35px;
  }
`;

const PrivacyPageComponent = ({ t, history }) => {
  //   useEffect(() => {
  //     setDocumentTitle(t("Common:About"));
  //   }, [t]);

  const onOpenEditorsPopup = () => {
    // if (localStorage.getItem("protocoldetector") == 1) {
    //   openCustomProtocolInIframe(customUrlForFileOpenDesktopEditor);
    // } else {
    //   window.open(urlForOpenPrivate, "_blank");
    // }

    const fileId = history.location.search.slice(1);
    window.open(
      `oo-office:${window.location.origin}/products/files/doceditor?${fileId}`,
      "_self"
    );
  };

  return (
    <StyledPrivacyPage>
      <div className="privacy-rooms-avatar">
        <Link href="/">
          <img
            className="privacy-rooms-logo"
            src="images/dark_general.png"
            width="320"
            height="181"
            alt="Logo"
          />
        </Link>
      </div>

      <div className="privacy-rooms-body">
        <Text
          textAlign="center"
          className="privacy-rooms-text-header"
          fontSize="38px"
        >
          {t("PrivacyHeader")}
        </Text>

        <Text as="div" textAlign="center" fontSize="20px" fontWeight={300}>
          <Trans t={t} i18nKey="PrivacyClick" ns="PrivacyPage">
            Click Open <strong>ONLYOFFICE Desktop</strong> in the browser dialog
            to work with the encrypted documents
          </Trans>
          .
        </Text>

        <Text
          textAlign="center"
          className="privacy-rooms-text-dialog"
          fontSize="20px"
          fontWeight={300}
        >
          {t("PrivacyDialog")}.
        </Text>
        <Button
          onClick={onOpenEditorsPopup}
          size="large"
          primary
          label={t("PrivacyButton")}
        />

        <label className="privacy-rooms-text-separator" />

        <div className="privacy-rooms-install">
          <Text
            className="privacy-rooms-install-text"
            fontSize="16px"
            fontWeight={300}
          >
            {t("PrivacyEditors")}?
          </Text>
          <Link
            className="privacy-rooms-link privacy-rooms-install-text"
            fontSize="16px"
            isHovered
            color="#116d9d"
            href="https://www.onlyoffice.com/desktop.aspx"
          >
            {t("PrivacyInstall")}
          </Link>
        </div>

        <Text
          as="div"
          color="#83888D"
          fontSize="12px"
          textAlign="center"
          className="privacy-rooms-text-description"
        >
          <p>{t("PrivacyDescriptionEditors")}.</p>
          <p>{t("PrivacyDescriptionConnect")}.</p>
        </Text>
      </div>
    </StyledPrivacyPage>
  );
};

const PrivacyPageWrapper = withTranslation(["PrivacyPage"])(
  withRouter(PrivacyPageComponent)
);

const PrivacyPage = (props) => {
  return (
    <I18nextProvider i18n={i18n}>
      <PageLayout>
        <PageLayout.SectionBody>
          <PrivacyPageWrapper {...props} />
        </PageLayout.SectionBody>
      </PageLayout>
    </I18nextProvider>
  );
};

export default PrivacyPage;
