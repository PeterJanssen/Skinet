using Microsoft.AspNetCore.Builder;

namespace API.Extensions
{
    public static class SecurityExtensions
    {
        public static IApplicationBuilder AddSecurityExtension(this IApplicationBuilder app)
        {
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opt => opt.NoReferrer());
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            app.UseXfo(opt => opt.Deny());
            app.UseCsp(opt => opt
            .BlockAllMixedContent()
            .StyleSources(s => s.Self().CustomSources(
                "https://fonts.googleapis.com"
            ).UnsafeInline())
            .FontSources(s => s.Self().CustomSources(
                "https://fonts.gstatic.com",
                "data:"
            ))
            .FormActions(s => s.Self())
            .FrameAncestors(s => s.Self())
            .ImageSources(s => s.Self().CustomSources(
                "https://res.cloudinary.com",
                "data:",
                "https://avatars3.githubusercontent.com"
                ))
            .ScriptSources(s => s.Self().CustomSources(
                "https://js.stripe.com/v3/",
                "https://apis.google.com/"
                ).UnsafeInline())
            );

            return app;
        }
    }
}