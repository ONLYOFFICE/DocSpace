import React from "react";
import Scrollbar from "@docspace/components/scrollbar";

const ArticleBody = ({ children }) => {
  return (
    <Scrollbar className="article-body__scrollbar" stype="mediumBlack">
      {children}
    </Scrollbar>
  );
};

ArticleBody.displayName = "Body";

export default React.memo(ArticleBody);
