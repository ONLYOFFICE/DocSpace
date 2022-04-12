import React from "react";
import Headline from "@appserver/common/components/Headline";

const SectionHeaderContent = ({ title }) => {
  return (
    <div>
      <Headline className="headline-header" type="content" truncate={true}>
        {title}
      </Headline>
    </div>
  );
};

export default React.memo(SectionHeaderContent);
