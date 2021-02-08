import styled from "styled-components";

import { Base } from "../../themes";
import { tablet } from "../../utils/device";

const StyledSaveCancelButtons = styled.div`
  display: flex;
  position: absolute;
  justify-content: space-between;
  box-sizing: border-box;
  align-items: center;

  bottom: ${(props) => props.theme.saveCancelButtons.bottom};
  width: ${(props) => props.theme.saveCancelButtons.width};

  left: ${(props) => props.theme.saveCancelButtons.left};
  padding: ${(props) => props.theme.saveCancelButtons.padding};

  .save-button {
    margin-right: ${(props) => props.theme.saveCancelButtons.marginRight};
  }

  .unsaved-changes {
    color: ${(props) => props.theme.saveCancelButtons.unsavedColor};
  }

  @media ${tablet} {
    justify-content: flex-end;
    position: fixed;

    .unsaved-changes {
      display: none;
    }
  }
`;
StyledSaveCancelButtons.defaultProps = { theme: Base };
export default StyledSaveCancelButtons;
