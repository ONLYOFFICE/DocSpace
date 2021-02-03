import styled, { css } from "styled-components";
import { DropDownItem } from "asc-web-components";

const commonStyle = css`
  font-family: "Open Sans", sans-serif, Arial;
  font-style: normal;
  color: #ffffff;
  margin-left: 60px;
  margin-top: -3px;
  max-width: 300px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

export const StyledProfileMenu = styled(DropDownItem)`
  position: relative;
  overflow: visible;
  padding: 0px;
  cursor: pointer;
  display: inline-block;
  margin-top: -6px;
`;

export const MenuContainer = styled.div`
  position: relative;
  height: 76px;
  background: linear-gradient(200.71deg, #2274aa 0%, #0f4071 100%);
  border-radius: 6px 6px 0px 0px;
  padding: 16px;
  cursor: default;
  box-sizing: border-box;
`;

export const AvatarContainer = styled.div`
  display: inline-block;
  float: left;
`;

export const MainLabelContainer = styled.div`
  font-size: 16px;
  line-height: 28px;

  ${commonStyle}
`;

export const LabelContainer = styled.div`
  font-weight: normal;
  font-size: 11px;
  line-height: 16px;

  ${commonStyle}
`;

export const TopArrow = styled.div`
  position: absolute;
  cursor: default;
  top: -6px;
  right: 16px;
  width: 24px;
  height: 6px;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M9.27954 1.12012C10.8122 -0.295972 13.1759 -0.295971 14.7086 1.12012L18.8406 4.93793C19.5796 5.62078 20.5489 6 21.5551 6H24H0H2.43299C3.4392 6 4.40845 5.62077 5.1475 4.93793L9.27954 1.12012Z' fill='%23206FA4'/%3E%3C/svg%3E");
`;
