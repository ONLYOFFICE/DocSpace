import styled from 'styled-components';
import { device } from 'asc-web-components'

const MainContainer = styled.div`
  display: flex;
  flex-direction: row;

  @media ${device.tablet} {
    flex-direction: column;
  }
`;

const AvatarContainer = styled.div`
  margin: 0 32px 32px 0;
  width: 160px;
`;

const MainFieldsContainer = styled.div`
  flex-grow: 1;
`;

export { MainContainer, AvatarContainer, MainFieldsContainer }