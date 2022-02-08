import { Base } from "@appserver/components/themes";
import styled from "styled-components";

const StyledDialogLoader = styled.div`
  .dialog-loader-header {
    border-bottom: ${(props) => props.theme.dialogLoader.borderBottom};
    display: flex;
    padding: 12px 0;
  }

  .dialog-loader-body {
    display: flex;
    flex-direction: column;
    justify-content: center;
    padding-top: 12px;
  }

  .dialog-loader-footer {
    display: flex;
    padding-top: 12px;
  }

  .dialog-loader-icon {
    margin-left: auto;
  }
`;

StyledDialogLoader.defaultProps = { theme: Base };

export default StyledDialogLoader;
