import styled from "styled-components";

const StyledFloatingButton = styled.div`
  position: relative;
  width: 54px;
  height: 54px;
  box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  border-radius: 54px;
  cursor: pointer;
  background: #fff;
  box-sizing: border-box;
  text-align: center;
  padding-top: 16px;
`;

const StyledAlertIcon = styled.div`
  position: absolute;
  width: 12px;
  height: 12px;
  left: 26px;
  top: 10px;
`;

export { StyledFloatingButton, StyledAlertIcon };
