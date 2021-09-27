import styled from 'styled-components';
import { tablet, mobile } from '@appserver/components/utils/device';

const StyledContainer = styled.div`
  width: 256px;
  padding: 0 20px;

  @media ${tablet} {
    width: 52px;
    padding: 0;
  }

  @media ${mobile} {
    width: 100%;
    padding: 0 16px;
  }
`;

const StyledBlock = styled.div`
  width: 100%;
  height: auto;

  margin-bottom: 20px;

  @media ${tablet} {
    margin-bottom: 24px;
  }
`;

export { StyledBlock, StyledContainer };
