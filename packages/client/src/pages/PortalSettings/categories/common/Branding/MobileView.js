import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

import MobileCategoryWrapper from "../../../components/MobileCategoryWrapper";

const StyledWrapper = styled.div`
  margin-top: 13px;
  display: flex;
  flex-direction: column;
`;

const MobileView = () => {
  const { t } = useTranslation(["Settings"]);
  const navigate = useNavigate();

  const onClickLink = (e) => {
    e.preventDefault();
    navigate(e.target.pathname);
  };

  return (
    <StyledWrapper>
      <MobileCategoryWrapper
        title={t("WhiteLabel")}
        subtitle={t("BrandingSubtitle")}
        url="/portal-settings/customization/branding/white-label"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("CompanyInfoSettings")}
        subtitle={t("BrandingSectionDescription")}
        url="/portal-settings/customization/branding/company-info-settings"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("AdditionalResources")}
        subtitle={t("AdditionalResourcesSubtitle")}
        url="/portal-settings/customization/branding/additional-resources"
        onClickLink={onClickLink}
      />
    </StyledWrapper>
  );
};

export default MobileView;
