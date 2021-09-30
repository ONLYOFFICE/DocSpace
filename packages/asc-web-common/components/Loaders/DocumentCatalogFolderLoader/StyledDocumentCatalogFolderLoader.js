import styled from 'styled-components';
import RectangleLoader from '../RectangleLoader';
import { tablet, mobile } from '@appserver/components/utils/device';

const StyledContainer = styled.div`
  margin: 0;
  width: 256px;
  padding: 0 20px;

  @media ${tablet} {
    width: ${(props) => (props.showText ? '240px' : '52px')};
    padding: ${(props) => (props.showText ? '0 16px' : '0')};
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

  margin-bottom: 20px;

  @media ${tablet} {
    margin-bottom: 24px;
  }
`;

const StyledRectangleLoader = styled(RectangleLoader)`
  height: 20px;
  padding: 1px 0 11px;

  @media ${tablet} {
    height: 41px;
    padding: 0;
  }
`;

export { StyledBlock, StyledContainer, StyledRectangleLoader };
