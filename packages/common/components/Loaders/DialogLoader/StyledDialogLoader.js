import { Base } from "@docspace/components/themes";
import styled from "styled-components";
import { mobile } from "@docspace/components/utils/device";

const StyledDialogLoader = styled.div`
  height: auto;
  width: ${(props) => (props.isLarge ? "520px" : "400px")};
  @media ${mobile} {
    width: 100%;
  }

  .dialog-loader-header {
    border-bottom: ${(props) =>
      `1px solid ${props.theme.modalDialog.headerBorderColor}`};
    padding: 12px 16px;
  }

  .dialog-loader-body {
    padding: 12px 16px 8px;
  }

  .dialog-loader-footer {
    ${(props) =>
      props.withFooterBorder &&
      `border-top: 1px solid ${props.theme.modalDialog.headerBorderColor}`};
    display: flex;
    gap: 10px;
    padding: 16px;
  }
`;

StyledDialogLoader.defaultProps = { theme: Base };

export default StyledDialogLoader;
