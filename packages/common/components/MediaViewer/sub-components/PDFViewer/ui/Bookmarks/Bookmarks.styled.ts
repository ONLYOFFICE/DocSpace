import styled from "styled-components";

export const List = styled.ul`
  padding-left: 16px;
  padding-right: 30px;

  list-style: none;
  margin-top: 0px;

  display: flex;
  flex-direction: column;
`;

export const Item = styled.li`
  color: #ffffff;
  padding: 1px 16px 0 16px;
  font-weight: 400;
  font-size: 13px;
  line-height: 20px;

  cursor: pointer;

  margin-top: -1px;

  @media (hover: hover) {
    :hover {
      background: #474747;
    }
  }
`;

export const Text = styled.p`
  margin: 0;
  border-bottom: 1px solid #474747;
  padding: 6px 0;
`;
