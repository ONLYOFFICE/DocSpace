import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

import MobileCategoryWrapper from "../../../components/MobileCategoryWrapper";

const StyledWrapper = styled.div`
  margin-top: 24px;
  display: flex;
  flex-direction: column;
  gap: 20px;
`;

const MobileView = () => {
  const { t } = useTranslation(["SingleSignOn", "Settings"]);
  const navigate = useNavigate();

  const onClickLink = (e) => {
    e.preventDefault();
    navigate(e.target.pathname);
  };

  return (
    <StyledWrapper>
      <MobileCategoryWrapper
        title={t("ServiceProviderSettings")}
        subtitle={t("ServiceProviderSettingsDescription")}
        url="/portal-settings/integration/single-sign-on/sp-settings"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("SpMetadata")}
        subtitle={t("SpMetadataDescription")}
        url="/portal-settings/integration/single-sign-on/sp-metadata"
        onClickLink={onClickLink}
      />
    </StyledWrapper>
  );
};

export default MobileView;
