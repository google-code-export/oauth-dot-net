<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" />
  <xsl:template name="substring-after-last">
    <xsl:param name="string" />
    <xsl:param name="delimiter" />
    <xsl:choose>
      <xsl:when test="contains($string, $delimiter)">
        <xsl:call-template name="substring-after-last">
          <xsl:with-param name="string"
            select="substring-after($string, $delimiter)" />
          <xsl:with-param name="delimiter" select="$delimiter" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$string" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="/">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="test-results">
    <xsl:text>Tests run: </xsl:text>
    <xsl:value-of select="@total"/>
    <xsl:text>, Failures: </xsl:text>
    <xsl:value-of select="@failures"/>
    <xsl:text>, Not run: </xsl:text>
    <xsl:value-of select="@not-run"/>
    <xsl:text>, Time: </xsl:text>
    <xsl:value-of select="test-suite/@time"/>
    <xsl:text> seconds
</xsl:text>
    <xsl:if test="//test-case[failure]">
      <xsl:apply-templates select="//test-case[failure]" />
    </xsl:if>
  </xsl:template>

  <xsl:template name="ErrorMessage">
    <xsl:param name="file" />
    <xsl:param name="line" />
    <xsl:param name="message" />
    <xsl:param name="testCase" />
    <xsl:value-of select="$file"/>(<xsl:value-of select="$line" />,1): error Nunit: <xsl:value-of select="$message"/> <xsl:value-of select="$testCase" />
  </xsl:template>

  <xsl:template match="test-case">
    <xsl:variable name="normalizedTrace" select ="normalize-space(failure/stack-trace)" />
    <xsl:variable name="normalizedMessage" select ="normalize-space(failure/message)" />
    <xsl:variable name="test">
      <xsl:call-template name="substring-after-last">
        <xsl:with-param name="text" select="@name" />
        <xsl:with-param name="delimiter" select="'.'" />
      </xsl:call-template>
    </xsl:variable>

    <xsl:if test="$normalizedTrace">
      <xsl:variable name="fileName" select="substring-before(substring-after($normalizedTrace,' in '), ':line ')" />
      <xsl:variable name="lineNumber" select="substring-before(substring-after($normalizedTrace,':line '), ' at')" />
      <xsl:if test="$lineNumber">
        <xsl:call-template name="ErrorMessage">
          <xsl:with-param name="file" select="$fileName" />
          <xsl:with-param name="line" select="$lineNumber" />
          <xsl:with-param name="message" select="$normalizedMessage" />
          <xsl:with-param name="testCase" select="$test" />
        </xsl:call-template>
      </xsl:if>
      <xsl:if test="not($lineNumber)">
        <!-- If we didn't find a line number, it's because there was no following stack trace.  Search again leaving out the "substring-before" call -->
        <xsl:variable name="realLineNumber" select="substring-after($normalizedTrace,':line ')" />
        <xsl:call-template name="ErrorMessage">
          <xsl:with-param name="file" select="$fileName" />
          <xsl:with-param name="line" select="$realLineNumber" />
          <xsl:with-param name="message" select="$normalizedMessage" />
          <xsl:with-param name="testCase" select="$test" />
        </xsl:call-template>
      </xsl:if>
    </xsl:if>
    <xsl:if test="not($normalizedTrace)">
      <xsl:call-template name="ErrorMessage">
        <xsl:with-param name="file">Unknown File</xsl:with-param>
        <xsl:with-param name="line">1</xsl:with-param>
        <xsl:with-param name="message" select="$normalizedMessage" />
        <xsl:with-param name="testCase" select="$test" />
      </xsl:call-template>
    </xsl:if>
    <xsl:text>
</xsl:text>
  </xsl:template>
</xsl:stylesheet>
