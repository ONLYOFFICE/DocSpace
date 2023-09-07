import RectangleLoader from "@docspace/common/components/Loaders/RectangleLoader";
import CircleLoader from "@docspace/common/components//Loaders/CircleLoader";
import styled from "styled-components";

const StyledLoader = styled.div`
  display: flex;
  flex-direction: column;
  gap: 24px;

  .user {
    display: flex;
    align-items: center;
    gap: 16px;
  }

  .name {
    display: flex;
    flex-direction: column;
    gap: 2px;
  }

  .avatar {
    width: 80px;
    height: 80px;
  }

  .new-owner_header {
    display: flex;
    flex-direction: column;
    gap: 4px;
    padding-bottom: 12px;
  }

  .new-owner_add {
    display: flex;
    align-items: center;
    gap: 12px;
  }

  .description {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }
`;

const Loader = () => {
  return (
    <StyledLoader>
      <div className="user">
        <CircleLoader className="avatar" radius="40" x="40" y="40" />

        <div className="name">
          <RectangleLoader width="154" height="20" />
          <RectangleLoader width="113" height="20" />
        </div>
      </div>

      <div className="new-owner">
        <div className="new-owner_header">
          <RectangleLoader width="113" height="16" />
          <RectangleLoader width="253" height="20" />
        </div>

        <div className="new-owner_add">
          <RectangleLoader width="32" height="32" />
          <RectangleLoader width="113" height="20" />
        </div>
      </div>

      <div className="description">
        <RectangleLoader height="40" />
        <RectangleLoader width="223" height="20" />
        <RectangleLoader width="160" height="20" />
      </div>
    </StyledLoader>
  );
};

export default Loader;
