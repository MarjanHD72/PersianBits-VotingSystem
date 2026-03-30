@page
@model Votingsystem.frontend.Pages.Admin.CreatePollModel
@{
    ViewData["Title"] = "Create Content";
}

@*
  CREATE CONTENT (Admin) - Frontend only
  - Election is the core feature (featured card)
  - Other types remain available but secondary
  - Quick Poll removed completely
*@

<section class="max-w-5xl mx-auto space-y-6">

    <!-- Header -->
    <div class="flex items-start justify-between gap-4">
        <div class="space-y-1">
            <h1 class="text-2xl font-semibold">Create content</h1>
            <p class="text-slate-400">Election is the main feature. Other question types are optional.</p>
        </div>

        <a asp-page="/Admin/Dashboard"
           class="px-4 py-3 rounded-xl border border-slate-800 hover:bg-slate-800/60">
            Back
        </a>
    </div>

    <!-- Featured: Election -->
    <a asp-page="/Admin/Builders/Election"
       class="group block rounded-3xl border border-[#D4AF37]/30 bg-[#D4AF37]/10 p-7 hover:bg-[#D4AF37]/15 transition">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
            <div class="space-y-2">
                <p class="text-xs uppercase tracking-wider text-[#D4AF37]/80">Core feature</p>
                <h2 class="text-2xl font-semibold">Election</h2>
                <p class="text-slate-300 max-w-2xl">
                    Create a candidate-based election with images and descriptions. Users can open each candidate
                    to read details before voting.
                </p>

                <div class="flex flex-wrap gap-2 pt-2">
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        Candidate profiles
                    </span>
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        One vote (or configurable later)
                    </span>
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        Clean user voting UI
                    </span>
                </div>
            </div>

            <div class="shrink-0">
                <span class="inline-flex items-center justify-center px-5 py-3 rounded-2xl font-semibold
                             bg-[#D4AF37] text-slate-950
                             group-hover:bg-[#FFC94A] transition
                             shadow-[0_0_30px_rgba(99,102,241,0.30)]">
                    Create Election
                </span>
            </div>
        </div>
    </a>

    <!-- Secondary types -->
    <div class="text-sm text-slate-400 pt-2">OTHER QUESTION TYPES</div>

    <div class="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">

        <!-- Multichoice -->
        <a asp-page="/Admin/Builders/Multichoice"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Multichoice</h3>
                    <p class="text-sm text-slate-400 mt-1">Select one or more choices from a list.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Text -->
        <a asp-page="/Admin/Builders/Text"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Text response</h3>
                    <p class="text-sm text-slate-400 mt-1">Collect open-ended written responses.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Rating scale -->
        <a asp-page="/Admin/Builders/RatingScale"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Rating scale</h3>
                    <p class="text-sm text-slate-400 mt-1">Users rate on a numeric scale.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Ranking -->
        <a asp-page="/Admin/Builders/Ranking"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Ranking</h3>
                    <p class="text-sm text-slate-400 mt-1">Rank options by preference.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

    </div>

    <p class="text-xs text-slate-500">
        Note: Builder pages are placeholders unless you create them. Next we’ll create the Election builder first.
    </p>

</section>@page
@model Votingsystem.frontend.Pages.Admin.CreatePollModel
@{
    ViewData["Title"] = "Create Content";
}

@*
  CREATE CONTENT (Admin) - Frontend only
  - Election is the core feature (featured card)
  - Other types remain available but secondary
  - Quick Poll removed completely
*@

<section class="max-w-5xl mx-auto space-y-6">

    <!-- Header -->
    <div class="flex items-start justify-between gap-4">
        <div class="space-y-1">
            <h1 class="text-2xl font-semibold">Create content</h1>
            <p class="text-slate-400">Election is the main feature. Other question types are optional.</p>
        </div>

        <a asp-page="/Admin/Dashboard"
           class="px-4 py-3 rounded-xl border border-slate-800 hover:bg-slate-800/60">
            Back
        </a>
    </div>

    <!-- Featured: Election -->
    <a asp-page="/Admin/Builders/Election"
       class="group block rounded-3xl border border-[#D4AF37]/30 bg-[#D4AF37]/10 p-7 hover:bg-[#D4AF37]/15 transition">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
            <div class="space-y-2">
                <p class="text-xs uppercase tracking-wider text-[#D4AF37]/80">Core feature</p>
                <h2 class="text-2xl font-semibold">Election</h2>
                <p class="text-slate-300 max-w-2xl">
                    Create a candidate-based election with images and descriptions. Users can open each candidate
                    to read details before voting.
                </p>

                <div class="flex flex-wrap gap-2 pt-2">
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        Candidate profiles
                    </span>
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        One vote (or configurable later)
                    </span>
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        Clean user voting UI
                    </span>
                </div>
            </div>

            <div class="shrink-0">
                <span class="inline-flex items-center justify-center px-5 py-3 rounded-2xl font-semibold
                             bg-[#D4AF37] text-slate-950
                             group-hover:bg-[#FFC94A] transition
                             shadow-[0_0_30px_rgba(99,102,241,0.30)]">
                    Create Election
                </span>
            </div>
        </div>
    </a>

    <!-- Secondary types -->
    <div class="text-sm text-slate-400 pt-2">OTHER QUESTION TYPES</div>

    <div class="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">

        <!-- Multichoice -->
        <a asp-page="/Admin/Builders/Multichoice"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Multichoice</h3>
                    <p class="text-sm text-slate-400 mt-1">Select one or more choices from a list.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Text -->
        <a asp-page="/Admin/Builders/Text"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Text response</h3>
                    <p class="text-sm text-slate-400 mt-1">Collect open-ended written responses.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Rating scale -->
        <a asp-page="/Admin/Builders/RatingScale"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Rating scale</h3>
                    <p class="text-sm text-slate-400 mt-1">Users rate on a numeric scale.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Ranking -->
        <a asp-page="/Admin/Builders/Ranking"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Ranking</h3>
                    <p class="text-sm text-slate-400 mt-1">Rank options by preference.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

    </div>

    <p class="text-xs text-slate-500">
        Note: Builder pages are placeholders unless you create them. Next we’ll create the Election builder first.
    </p>

</section>@page
@model Votingsystem.frontend.Pages.Admin.CreatePollModel
@{
    ViewData["Title"] = "Create Content";
}

@*
  CREATE CONTENT (Admin) - Frontend only
  - Election is the core feature (featured card)
  - Other types remain available but secondary
  - Quick Poll removed completely
*@

<section class="max-w-5xl mx-auto space-y-6">

    <!-- Header -->
    <div class="flex items-start justify-between gap-4">
        <div class="space-y-1">
            <h1 class="text-2xl font-semibold">Create content</h1>
            <p class="text-slate-400">Election is the main feature. Other question types are optional.</p>
        </div>

        <a asp-page="/Admin/Dashboard"
           class="px-4 py-3 rounded-xl border border-slate-800 hover:bg-slate-800/60">
            Back
        </a>
    </div>

    <!-- Featured: Election -->
    <a asp-page="/Admin/Builders/Election"
       class="group block rounded-3xl border border-[#D4AF37]/30 bg-[#D4AF37]/10 p-7 hover:bg-[#D4AF37]/15 transition">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
            <div class="space-y-2">
                <p class="text-xs uppercase tracking-wider text-[#D4AF37]/80">Core feature</p>
                <h2 class="text-2xl font-semibold">Election</h2>
                <p class="text-slate-300 max-w-2xl">
                    Create a candidate-based election with images and descriptions. Users can open each candidate
                    to read details before voting.
                </p>

                <div class="flex flex-wrap gap-2 pt-2">
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        Candidate profiles
                    </span>
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        One vote (or configurable later)
                    </span>
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        Clean user voting UI
                    </span>
                </div>
            </div>

            <div class="shrink-0">
                <span class="inline-flex items-center justify-center px-5 py-3 rounded-2xl font-semibold
                             bg-[#D4AF37] text-slate-950
                             group-hover:bg-[#FFC94A] transition
                             shadow-[0_0_30px_rgba(99,102,241,0.30)]">
                    Create Election
                </span>
            </div>
        </div>
    </a>

    <!-- Secondary types -->
    <div class="text-sm text-slate-400 pt-2">OTHER QUESTION TYPES</div>

    <div class="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">

        <!-- Multichoice -->
        <a asp-page="/Admin/Builders/Multichoice"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Multichoice</h3>
                    <p class="text-sm text-slate-400 mt-1">Select one or more choices from a list.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Text -->
        <a asp-page="/Admin/Builders/Text"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Text response</h3>
                    <p class="text-sm text-slate-400 mt-1">Collect open-ended written responses.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Rating scale -->
        <a asp-page="/Admin/Builders/RatingScale"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Rating scale</h3>
                    <p class="text-sm text-slate-400 mt-1">Users rate on a numeric scale.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Ranking -->
        <a asp-page="/Admin/Builders/Ranking"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Ranking</h3>
                    <p class="text-sm text-slate-400 mt-1">Rank options by preference.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

    </div>

    <p class="text-xs text-slate-500">
        Note: Builder pages are placeholders unless you create them. Next we’ll create the Election builder first.
    </p>

</section>@page
@model Votingsystem.frontend.Pages.Admin.CreatePollModel
@{
    ViewData["Title"] = "Create Content";
}

@*
  CREATE CONTENT (Admin) - Frontend only
  - Election is the core feature (featured card)
  - Other types remain available but secondary
  - Quick Poll removed completely
*@

<section class="max-w-5xl mx-auto space-y-6">

    <!-- Header -->
    <div class="flex items-start justify-between gap-4">
        <div class="space-y-1">
            <h1 class="text-2xl font-semibold">Create content</h1>
            <p class="text-slate-400">Election is the main feature. Other question types are optional.</p>
        </div>

        <a asp-page="/Admin/Dashboard"
           class="px-4 py-3 rounded-xl border border-slate-800 hover:bg-slate-800/60">
            Back
        </a>
    </div>

    <!-- Featured: Election -->
    <a asp-page="/Admin/Builders/Election"
       class="group block rounded-3xl border border-[#D4AF37]/30 bg-[#D4AF37]/10 p-7 hover:bg-[#D4AF37]/15 transition">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
            <div class="space-y-2">
                <p class="text-xs uppercase tracking-wider text-[#D4AF37]/80">Core feature</p>
                <h2 class="text-2xl font-semibold">Election</h2>
                <p class="text-slate-300 max-w-2xl">
                    Create a candidate-based election with images and descriptions. Users can open each candidate
                    to read details before voting.
                </p>

                <div class="flex flex-wrap gap-2 pt-2">
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        Candidate profiles
                    </span>
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        One vote (or configurable later)
                    </span>
                    <span class="text-xs px-2 py-1 rounded-full border border-[#D4AF37]/30 text-[#D4AF37]">
                        Clean user voting UI
                    </span>
                </div>
            </div>

            <div class="shrink-0">
                <span class="inline-flex items-center justify-center px-5 py-3 rounded-2xl font-semibold
                             bg-[#D4AF37] text-slate-950
                             group-hover:bg-[#FFC94A] transition
                             shadow-[0_0_30px_rgba(99,102,241,0.30)]">
                    Create Election
                </span>
            </div>
        </div>
    </a>

    <!-- Secondary types -->
    <div class="text-sm text-slate-400 pt-2">OTHER QUESTION TYPES</div>

    <div class="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">

        <!-- Multichoice -->
        <a asp-page="/Admin/Builders/Multichoice"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Multichoice</h3>
                    <p class="text-sm text-slate-400 mt-1">Select one or more choices from a list.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Text -->
        <a asp-page="/Admin/Builders/Text"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Text response</h3>
                    <p class="text-sm text-slate-400 mt-1">Collect open-ended written responses.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Rating scale -->
        <a asp-page="/Admin/Builders/RatingScale"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Rating scale</h3>
                    <p class="text-sm text-slate-400 mt-1">Users rate on a numeric scale.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

        <!-- Ranking -->
        <a asp-page="/Admin/Builders/Ranking"
           class="group rounded-2xl border border-slate-800 bg-slate-900/40 p-5 hover:bg-slate-900/70 transition">
            <div class="flex items-start justify-between gap-4">
                <div>
                    <h3 class="text-lg font-semibold">Ranking</h3>
                    <p class="text-sm text-slate-400 mt-1">Rank options by preference.</p>
                </div>
                <span class="text-xs px-2 py-1 rounded-full border border-slate-700 text-slate-300">
                    Open
                </span>
            </div>
        </a>

    </div>

    <p class="text-xs text-slate-500">
        Note: Builder pages are placeholders unless you create them.  Next we’ll create the Election builder first.
    </p>

</section>