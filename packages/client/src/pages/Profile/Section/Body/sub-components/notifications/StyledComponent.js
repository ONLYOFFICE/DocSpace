import styled, { css } from "styled-components";
import { mobile, tablet } from "@docspace/components/utils/device";

const StyledSectionBodyContent = styled.div`
  width: 100%;
  max-width: 660px;

  @media ${tablet} {
    max-width: 100%;
  }

  .notification-container {
    display: grid;
    grid-template-columns: 1fr 124px;
    margin-bottom: 12px;

    .toggle-btn {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              padding-right: 44px;
            `
          : css`
              padding-left: 44px;
            `}
    }
    .notification-container_description {
      color: ${(props) =>
        props.theme.profile.notifications.textDescriptionColor};
    }
  }
  .badges-container {
    margin-bottom: 24px;
  }
`;

const StyledTextContent = styled.div`
  margin-bottom: 12px;
  height: 24px;
  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};

  p {
    line-height: 16px;
    padding-bottom: 8px;
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
