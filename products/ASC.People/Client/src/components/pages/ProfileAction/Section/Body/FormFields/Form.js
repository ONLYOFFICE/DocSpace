import styled from 'styled-components';
import { utils } from 'asc-web-components'

const MainContainer = styled.div`
  display: flex;
  flex-direction: row;

  .field-input {
    width: 100%;
    max-width: 320px;
  }

  .radio-group {
    line-height: 32px;
    display: flex;

    label:not(:first-child) {
        margin-left: 33px;
    }
  }

  .departments-field {
    position: relative;
    margin: 0 0 40px 0;
    max-width: 835px;

    .department-add-btn {
      margin: 0 8px 8px 0;
      float: left;
    }
    .department-item {
      margin: 0 8px 8px 0;
    }
  }

  @media ${utils.device.tablet} {
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