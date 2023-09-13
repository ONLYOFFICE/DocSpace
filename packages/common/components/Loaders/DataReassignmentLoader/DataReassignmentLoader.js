import RectangleLoader from "@docspace/common/components/Loaders/RectangleLoader";
import CircleLoader from "@docspace/common/components//Loaders/CircleLoader";
import StyledDataReassignmentLoader from "./StyledDataReassignmentLoader";

const DataReassignmentLoader = () => {
  return (
    <StyledDataReassignmentLoader>
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
    </StyledDataReassignmentLoader>
  );
};

export default DataReassignmentLoader;
