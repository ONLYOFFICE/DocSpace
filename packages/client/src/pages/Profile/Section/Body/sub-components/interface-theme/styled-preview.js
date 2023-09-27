import styled, { css } from "styled-components";
import { tablet } from "@docspace/components/utils/device";

export const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  border: ${(props) => props.theme.profile.themePreview.border};
  border-radius: 12px;
  height: 284px;
  width: 318px;
  overflow: hidden;

  @media ${tablet} {
    width: 100%;
  }

  .card-header {
    padding: 11px 19px;
    border-bottom: ${(props) => props.theme.profile.themePreview.border};
    line-height: 20px;
  }

  .floating-btn {
    bottom: 100px !important;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            left: 30px !important;
          `
        : css`
            right: 30px !important;
          `}
  }
`;
