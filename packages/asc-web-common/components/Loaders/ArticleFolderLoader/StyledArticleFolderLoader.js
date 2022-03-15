import styled from "styled-components";
import RectangleLoader from "../RectangleLoader";
import { tablet, mobile } from "@appserver/components/utils/device";

const StyledContainer = styled.div`
  margin: 0;

  max-width: 216px;
  padding: 0 20px;

  @media ${tablet} {
    width: ${(props) => (props.showText ? "240px" : "52px")};
    padding: ${(props) => (props.showText ? "0 16px" : "10px 16px")};
  }

  @media ${mobile} {
    width: 100%;
    padding: 0 16px;
  }
`;

const StyledBlock = styled.div`
  margin: 0;
  width: 100%;
  height: auto;
  display: flex;
  flex-direction: column;
  margin-bottom: 20px;

  @media ${tablet} {
    margin-bottom: 24px;
  }
`;

const StyledRectangleLoader = styled(RectangleLoader)`
  height: 20px;
  width: 216px;
  padding: 0 0 16px;

  @media ${tablet} {
    height: 20px;
    width: 20px;
    padding: 0 0 24px;
  }
`;

export { StyledBlock, StyledContainer, StyledRectangleLoader };
