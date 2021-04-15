import styled from "styled-components";
import { mobile } from "@appserver/components/utils/device";

const StyledTile = styled.div`
  position: relative;
  display: inline-block;
  min-width: 220px;
  width: 100%;
  height: ${(props) => (props.isFolder ? "57px" : "240px")};
  border: 1px solid #d0d5da;
  border-radius: 3px;

  ${(props) =>
    props.isFolder
      ? `   
  &:before {
        content: "";
        position: absolute;
        top: -7px;
        left: -1px;
        border: 1px solid #d0d5da;
        border-top-left-radius: 3px;
        border-top-right-radius: 6px;
        width: 50px;
        height: 6px;
        background-color: #fff;
        border-bottom: transparent;
      }`
      : null}

  @media ${mobile} {
    width: 100%;
  }
`;

const StyledMainContent = styled.div`
  padding: 12px 12px 4px 12px;
  height: 175px;
`;

const StyledBottom = styled.div`
  display: grid;
  grid-template-columns: 24px auto;
  grid-gap: 8px;
  padding: ${(props) => (props.isFolder ? "20px 12px" : "8px 12px 12px 12px")};

  .first-content {
    display: inline-block;
    ${(props) =>
      !props.isFolder
        ? `
    width: 24px;
    height: 30px;`
        : `
    width: 16px;
    height: 16px;`}
  }
  .second-content {
    width: 100%;
    height: ${(props) => (props.isFolder ? "16px" : "30px")};
  }
`;

export { StyledTile, StyledBottom, StyledMainContent };
