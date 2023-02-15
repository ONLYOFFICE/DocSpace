import styled, { css } from "styled-components";
import { tablet } from "@docspace/components/utils/device";
import { isMobile } from "react-device-detect";

const StyledSettings = styled.div`
  margin-top: ${(props) =>
    props.hideAdminSettings ? 22 : props.showTitle ? 24 : 34}px;

  ${(props) =>
    props.hideAdminSettings &&
    css`
      padding-top: 2px;
    `}

  @media ${tablet} {
    margin-top: ${(props) => (props.hideAdminSettings ? 0 : 8)}px;
    ${(props) =>
      props.hideAdminSettings &&
      css`
        padding-top: 8px;
      `}
  }

  ${isMobile &&
  css`
    margin-top: 8px;
  `}

  width: 100%;

  display: grid;
  grid-gap: 32px;

  .toggle-btn {
    position: relative;
  }

  .heading {
    margin-bottom: -2px;
    margin-top: 0;
  }

  .toggle-button-text {
    margin-top: -1px;
  }

  .settings-section {
    display: grid;
    grid-gap: 18px;
  }
`;

export default StyledSettings;
