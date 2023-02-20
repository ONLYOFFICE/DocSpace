import styled from "styled-components";
import { Base } from "@docspace/components/themes";

const ControlBtn = styled.div`
  display: inline-block;
  height: 30px;
  line-height: 25px;
  margin: 5px;
  width: 40px;
  border-radius: 2px;
  cursor: pointer;
  text-align: center;

  &:hover {
    background-color: ${(props) =>
      props.theme.mediaViewer.controlBtn.backgroundColor};
  }
`;

ControlBtn.defaultProps = { theme: Base };

export default ControlBtn;
