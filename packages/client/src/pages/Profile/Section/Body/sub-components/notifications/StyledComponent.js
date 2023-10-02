import styled, { css } from "styled-components";
import { mobile, tablet } from "@docspace/components/utils/device";

const StyledSectionBodyContent = styled.div`
  width: 100%;
  max-width: 660px;

  @media ${tablet} {
    max-width: 100%;
  }

  .notification-container {
    display: flex;
    flex-direction: column;

    .row {
      display: flex;
      justify-content: space-between;
      align-items: center;

      div > label {
        position: relative;
        gap: 0;
      }
    }
    margin-bottom: 12px;

    .notification-container_description {
      color: ${(props) =>
        props.theme.profile.notifications.textDescriptionColor};
    }
  }

  .badges-container {
    margin-bottom: 24px;
    p {
      line-height: 20px;
    }
  }
`;

const StyledTextContent = styled.div`
  margin-bottom: 12px;
  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};

  p {
    line-height: 16px;
    padding-bottom: 8px;
  }

  .email-title {
    padding-top: 2px;
  }
`;

const StyledSectionHeader = styled.div`
  display: flex;
  align-items: center;
  .arrow-button {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 16px;
            transform: scaleX(-1);
          `
        : css`
            margin-right: 16px;
          `}
  }
`;
export { StyledTextContent, StyledSectionBodyContent, StyledSectionHeader };
