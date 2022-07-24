import React from "react";
import Headline from "@docspace/common/components/Headline";

const SectionHeaderContent = ({ title }) => {
  return (
    <div>
      <Headline
        fontSize="18px"
        className="headline-header"
        type="content"
        truncate={true}
      >
        {title}
      </Headline>
    </div>
  );
};

export default React.memo(SectionHeaderContent);
