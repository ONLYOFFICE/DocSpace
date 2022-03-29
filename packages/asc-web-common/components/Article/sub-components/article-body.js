import React from "react";

const ArticleBody = ({ children }) => {
  return <> {children}</>;
};

ArticleBody.displayName = "Body";

export default React.memo(ArticleBody);
